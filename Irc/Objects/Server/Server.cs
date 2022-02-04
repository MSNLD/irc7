using System.Reflection;
using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Interfaces;
using Irc.IO;
using Irc.Security;
using Irc7d;

namespace Irc.Objects.Server;

public class Server : ChatObject
{
    protected readonly IDataStore _dataStore;
    private readonly IFloodProtectionManager _floodProtectionManager;
    private readonly ISecurityManager _securityManager;
    private readonly ISocketServer _socketServer;
    public readonly ICommandCollection Commands;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _processingTask;
    public IDictionary<EnumProtocolType, IProtocol> _protocols = new Dictionary<EnumProtocolType, IProtocol>();
    public IList<IChannel> Channels;
    public DateTime CreationDate => _dataStore.GetAs<DateTime>("creation");

    public IList<User> Users = new List<User>();

    public Server(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands) : base(dataStore)
    {
        Name = "TK2CHATCHATA01";
        _socketServer = socketServer;
        _securityManager = securityManager;
        _floodProtectionManager = floodProtectionManager;
        _dataStore = dataStore;
        Channels = channels;
        Commands = commands;
        Name = Name;
        _processingTask = new Task(Process);
        _processingTask.Start();

        _dataStore.SetAs("creation", DateTime.UtcNow);
        _dataStore.Set("supported.channel.modes", (new ChannelModes()).GetSupportedModes() + (new MemberModes()).GetSupportedModes());
        _dataStore.Set("supported.user.modes", (new UserModes()).GetSupportedModes());

        _protocols.Add(EnumProtocolType.IRC, new Irc());

        socketServer.OnClientConnecting += (sender, connection) =>
        {
            // TODO: Need to start a new user out with protocol, below code is unreliable
            var user = CreateUser(connection);
            AddUser(user);
            user.Address.RemoteIP = connection.GetAddress();

            connection.OnConnect += (o, integer) => { Console.WriteLine("Connect"); };
            connection.OnReceive += (o, s) =>
            {
                //Console.WriteLine("OnRecv:" + s);
            };
            connection.Accept();
        };
        socketServer.Listen();
    }

    // Server Properties To be moved to another class later
    public bool AnnonymousAllowed { get; }
    public int ChannelCount { get; }
    public IList<ChatObject> IgnoredUsers { get; }
    public IList<string> Info { get; }
    public int MaxMessageLength { get; } = 512;
    public int MaxInputBytes { get; } = 512;
    public int MaxOutputBytes { get; } = 4096;
    public int NetInvisibleCount { get; }
    public int NetServerCount { get; }
    public int NetUserCount { get; }
    public string SecurityPackages => _securityManager.GetSupportedPackages();
    public int SysopCount { get; }
    public int UnknownConnectionCount => _socketServer.CurrentConnections - NetUserCount;
    public string RemoteIP { set; get; }

    private void Process()
    {
        var backoffMS = 0;
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var hasWork = false;

            // do stuff
            foreach (var user in Users)
            {
                if (user.DisconnectIfIncomingThresholdExceeded()) continue;

                if (user.GetDataRegulator().GetIncomingBytes() > 0)
                {
                    hasWork = true;
                    backoffMS = 0;

                    ProcessNextCommand(user);
                }

                if (!user.DisconnectIfOutgoingThresholdExceeded()) user.Flush();
            }

            if (!hasWork)
            {
                if (backoffMS < 1000) backoffMS += 10;
                Thread.Sleep(backoffMS);
            }
        }
    }

    private void ProcessNextCommand(User user)
    {
        var message = user.GetDataRegulator().PeekIncoming();
        var command = message.GetCommand();
        if (command != null)
        {
            var floodResult = _floodProtectionManager.Audit(user.GetFloodProtectionProfile(),
                command.GetDataType(), user.Level);
            if (floodResult == EnumFloodResult.Ok)
            {
                user.GetDataRegulator().PopIncoming();
                Console.WriteLine($"Processing: {message.OriginalText}");

                var chatFrame = new ChatFrame(this, user, message);
                if (command.CheckParameters(chatFrame)) command.Execute(chatFrame);
                else user.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(this, user, command.GetName()));

                // Check if user can register
                if (!chatFrame.User.Registered) Register.Execute(chatFrame);
            }
        }
        else
        {
            user.GetDataRegulator().PopIncoming();
            user.Send(Raw.IRCX_ERR_UNKNOWNCOMMAND_421(this, user, message.GetCommandName()));
            // command not found
        }
    }

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void RemoveUser(User user)
    {
        Users.Remove(user);
    }

    public void AddChannel(IChannel channel)
    {
        Channels.Add(channel);
    }

    public void RemoveChannel(IChannel channel)
    {
        Channels.Remove(channel);
    }

    public IChannel CreateChannel(string name)
    {
        return new Channel.Channel(name, new ChannelModes(), new DataStore(name, "store", true));
    }

    public User CreateUser(IConnection connection)
    {
        return new User(connection, _protocols.First().Value, new DataRegulator(MaxInputBytes, MaxOutputBytes),
            new FloodProtectionProfile(), new DataStore(Name, "store"), new UserModes(), this);
    }

    public IList<User> GetUsers()
    {
        return Users;
    }

    public IList<IChannel> GetChannels()
    {
        return Channels;
    }

    public string GetSupportedChannelModes() => _dataStore.Get("supported.channel.modes");
    public string GetSupportedUserModes() => _dataStore.Get("supported.user.modes");

    public Version GetVersion() => Assembly.GetExecutingAssembly().GetName().Version;

    public IDataStore GetDataStore() => _dataStore;

    public IChannel GetChannelByName(string name)
    {
        return Channels.SingleOrDefault(c =>
            string.Equals(c.GetName(), name, StringComparison.InvariantCultureIgnoreCase));
    }

    public IProtocol GetProtocol(EnumProtocolType protocolType)
    {
        if (_protocols.TryGetValue(protocolType, out var protocol)) return protocol;
        return null;
    }

    public ISecurityManager GetSecurityManager()
    {
        return _securityManager;
    }

    public ICredentialProvider GetCredentialManager()
    {
        return null;
    }

    public void Shutdown()
    {
        _cancellationTokenSource.Cancel();
        _processingTask.Wait();
    }
}