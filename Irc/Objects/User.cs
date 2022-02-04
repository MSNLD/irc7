using System.Collections.Concurrent;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Irc.Enumerations;
using Irc.Extensions.Security.Packages;
using Irc.Interfaces;
using Irc.Worker;
using Irc.Worker.Ircx.Objects;
using Irc7d;

namespace Irc.Objects;

public class User: ChatItem
{
    public Server Server { get; }

    //public Access Access;
    private readonly IConnection _connection;
    private readonly IDataRegulator _dataRegulator;
    private readonly IFloodProtectionProfile _floodProtectionProfile;
    public event EventHandler<string> OnSend;
    public IDictionary<IChannel, IChannelMember> Channels;
    public EnumUserAccessLevel Level;
    private IProtocol _protocol;
    private ISupportPackage _supportPackage;
    public IUserModeCollection Modes;
    public Address Address = new();
    
    public bool Guest;
    public bool Registered;
    public bool Authenticated;
    public string Client;
    public long LoggedOn;
    public long LastIdle;

    public User(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator, IFloodProtectionProfile floodProtectionProfile, IObjectStore objectStore, IPropStore propStore, IUserModeCollection modes, Server server): base(objectStore, propStore)
    {
        Server = server;
        _connection = connection;
        _protocol = protocol;
        _dataRegulator = dataRegulator;
        _floodProtectionProfile = floodProtectionProfile;
        _supportPackage = new ANON();
        Modes = modes;
        Channels = new ConcurrentDictionary<IChannel, IChannelMember>();

        _connection.OnReceive += (sender, s) =>
        {
            var message = new Message(_protocol, s);
            dataRegulator.PushIncoming(message);
        };
    }

    public void BroadcastToChannels(string data, bool ExcludeUser)
    {
        foreach (IChannel channel in Channels.Keys)
        {
            channel.Send(data, this, ExcludeUser);
        }
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

    public void Send(string message)
    {
        _dataRegulator.PushOutgoing(message);
    }

    public void Flush()
    {
        int totalBytes = _dataRegulator.GetOutgoingBytes();
        
        if (_dataRegulator.GetOutgoingBytes() > 0)
        {
            // Compensate for \r\n
            int queueLength = _dataRegulator.GetOutgoingQueueLength();
            int adjustedTotalBytes = totalBytes + (queueLength * 2);

            StringBuilder stringBuilder = new StringBuilder((int)adjustedTotalBytes);
            for (int i = 0; i < queueLength; i++)
            {
                stringBuilder.Append(_dataRegulator.PopOutgoing());
                stringBuilder.Append("\r\n");
            }
            Console.WriteLine($"Sending: {stringBuilder}");
            _connection?.Send(stringBuilder.ToString());
        }
    }

    public void Disconnect(string message)
    {
        _connection?.Disconnect($"{message}\r\n");
    }

    public IDataRegulator GetDataRegulator() => _dataRegulator;
    public IFloodProtectionProfile GetFloodProtectionProfile() => _floodProtectionProfile;
    public ISupportPackage GetSupportPackage() => _supportPackage;
    public void SetSupportPackage(ISupportPackage supportPackage) => _supportPackage = supportPackage;
    public void SetProtocol(IProtocol protocol) => _protocol = protocol;
    public IProtocol GetProtocol() => _protocol;
    public bool IsGuest() => Guest;
    public bool IsRegistered() => Registered;
    public bool IsAuthenticated() => Authenticated;
    public bool IsAnon() => (_supportPackage is ANON);

    public bool DisconnectIfOutgoingThresholdExceeded()
    {
        if (this.GetDataRegulator().IsOutgoingThresholdExceeded())
        {
            this.GetDataRegulator().Purge();
            this.Disconnect("Output quota exceeded");
            return true;
        }

        return false;
    }

    public bool DisconnectIfIncomingThresholdExceeded()
    {
        // Disconnect user if incoming quota exceeded
        if (this.GetDataRegulator().IsIncomingThresholdExceeded())
        {
            this.GetDataRegulator().Purge();
            this.Disconnect("Input quota exceeded");
            return true;
        }

        return false;
    }
}