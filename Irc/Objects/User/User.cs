using System.Collections.Concurrent;
using System.Text;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Security.Packages;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Objects;

public class User : ChatObject, IUser
{
    //public Access Access;
    private readonly IConnection _connection;
    private readonly IDataRegulator _dataRegulator;
    private readonly IDataStore _dataStore;
    private readonly IFloodProtectionProfile _floodProtectionProfile;
    private bool _authenticated;

    private bool _guest;
    private EnumUserAccessLevel _level;
    private IProtocol _protocol;
    private bool _registered;
    private ISupportPackage _supportPackage;
    private Address address = new();
    public IDictionary<IChannel, IChannelMember> Channels;
    public string Client;

    public long LastIdle = DateTime.UtcNow.Ticks;
    public long PingCount;
    public long LoggedOn;

    public User(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes,
        IServer server) : base(modes, dataStore)
    {
        Server = server;
        _connection = connection;
        _protocol = protocol;
        _dataRegulator = dataRegulator;
        _floodProtectionProfile = floodProtectionProfile;
        _dataStore = dataStore;
        _supportPackage = new ANON();
        Channels = new ConcurrentDictionary<IChannel, IChannelMember>();

        _connection.OnReceive += (sender, s) =>
        {
            LastIdle = DateTime.UtcNow.Ticks;
            PingCount = 0;
            var message = new Message(_protocol, s);
            dataRegulator.PushIncoming(message);
        };

        Address.SetIP(connection.GetAddress());
    }

    public IServer Server { get; }
    public event EventHandler<string> OnSend;

    public void BroadcastToChannels(string data, bool ExcludeUser)
    {
        foreach (var channel in Channels.Keys) channel.Send(data, this);
    }

    public void AddChannel(IChannel channel, IChannelMember member)
    {
        Channels.Add(channel, member);
    }

    public void RemoveChannel(IChannel channel)
    {
        Channels.Remove(channel);
    }

    public KeyValuePair<IChannel, IChannelMember> GetChannelMemberInfo(IChannel channel)
    {
        return Channels.FirstOrDefault(c => c.Key == channel);
    }

    public KeyValuePair<IChannel, IChannelMember> GetChannelInfo(string Name)
    {
        return Channels.FirstOrDefault(c => c.Key.GetName() == Name);
    }

    public IDictionary<IChannel, IChannelMember> GetChannels() => Channels;

    public override void Send(string message)
    {
        _dataRegulator.PushOutgoing(message);
    }

    public void Flush()
    {
        var totalBytes = _dataRegulator.GetOutgoingBytes();

        if (_dataRegulator.GetOutgoingBytes() > 0)
        {
            // Compensate for \r\n
            var queueLength = _dataRegulator.GetOutgoingQueueLength();
            var adjustedTotalBytes = totalBytes + queueLength * 2;

            var stringBuilder = new StringBuilder(adjustedTotalBytes);
            for (var i = 0; i < queueLength; i++)
            {
                stringBuilder.Append(_dataRegulator.PopOutgoing());
                stringBuilder.Append("\r\n");
            }

            Console.WriteLine($"Sending[{_protocol.GetType().Name}]: {stringBuilder}");
            _connection?.Send(stringBuilder.ToString());
        }
    }

    public void Disconnect(string message)
    {
        _connection?.Disconnect($"{message}\r\n");
    }

    public IDataRegulator GetDataRegulator()
    {
        return _dataRegulator;
    }

    public IFloodProtectionProfile GetFloodProtectionProfile()
    {
        return _floodProtectionProfile;
    }

    public ISupportPackage GetSupportPackage()
    {
        return _supportPackage;
    }

    public void SetSupportPackage(ISupportPackage supportPackage)
    {
        _supportPackage = supportPackage;
    }

    public void SetProtocol(IProtocol protocol)
    {
        _protocol = protocol;
    }

    public IProtocol GetProtocol()
    {
        return _protocol;
    }

    public IConnection GetConnection()
    {
        return _connection;
    }

    public EnumUserAccessLevel GetLevel()
    {
        return _level;
    }
    public override EnumUserAccessLevel Level {
        get
        {
            return GetLevel();
        }
    
    }

    public Address Address { get => address; set => address = value; }

    public Address GetAddress()
    {
        return Address;
    }

    public bool IsGuest()
    {
        return _guest;
    }

    public bool IsRegistered()
    {
        return _registered;
    }

    public bool IsAuthenticated()
    {
        return _authenticated;
    }

    public bool IsOn(IChannel channel)
    {
        return Channels.ContainsKey(channel);
    }

    public bool IsAnon()
    {
        return _supportPackage is ANON;
    }

    public bool IsSysop() => GetModes().GetModeChar(Resources.UserModeOper) == 1;
    public bool IsAdministrator() => GetModes().HasMode('a') && GetModes().GetModeChar(Resources.UserModeAdmin) == 1;

    public void PromoteToAdministrator()
    {
        var mode = Modes[Resources.UserModeAdmin];
        mode.Set(true);
        mode.DispatchModeChange(this, this, true);
        _level = EnumUserAccessLevel.Administrator;
        Send(Raw.IRCX_RPL_YOUREADMIN_386(Server, this));
    }

    public void PromoteToSysop()
    {
        var mode = Modes[Resources.UserModeOper];
        mode.Set(true);
        mode.DispatchModeChange(this, this, true);
        _level = EnumUserAccessLevel.Sysop;
        Send(Raw.IRCX_RPL_YOUREOPER_381(Server, this));
    }

    public void PromoteToGuide()
    {
        var mode = Modes[Resources.UserModeOper];
        mode.Set(true);
        mode.DispatchModeChange(this, this, true);
        _level = EnumUserAccessLevel.Guide;
        Send(Raw.IRCX_RPL_YOUREGUIDE_629(Server, this));
    }

    public bool DisconnectIfOutgoingThresholdExceeded()
    {
        if (GetDataRegulator().IsOutgoingThresholdExceeded())
        {
            GetDataRegulator().Purge();
            Disconnect("Output quota exceeded");
            return true;
        }

        return false;
    }

    public bool DisconnectIfIncomingThresholdExceeded()
    {
        // Disconnect user if incoming quota exceeded
        if (GetDataRegulator().IsIncomingThresholdExceeded())
        {
            GetDataRegulator().Purge();
            Disconnect("Input quota exceeded");
            return true;
        }

        return false;
    }

    public static long PING_INTERVAL = 180;
    public static long PING_MAX_ATTEMPTS = 3;
    public void DisconnectIfInactive()
    {
        var seconds = (DateTime.UtcNow.Ticks - LastIdle) / TimeSpan.TicksPerSecond;
        if (seconds > ((PingCount + 1) * PING_INTERVAL))
        {
            if (PingCount < PING_MAX_ATTEMPTS)
            {
                Console.WriteLine($"Ping Count for {this} hit stage {PingCount+1}");
                PingCount++;
                Send(Raw.RPL_PING(Server, this));
            }
            else
            {
                GetDataRegulator().Purge();
                Disconnect(Raw.IRCX_CLOSINGLINK_011_PINGTIMEOUT(Server, this, _connection.GetAddress()));
            }
        }
    }

    public void Register()
    {
        _registered = true;
    }

    public void Authenticate()
    {
        _authenticated = true;
    }

    public IDataStore GetDataStore()
    {
        return _dataStore;
    }

    public virtual bool CanBeModifiedBy(ChatObject source)
    {
        return source == this;
    }
}