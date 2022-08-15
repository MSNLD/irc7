using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Irc.Commands;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Security;
using Irc7d;

namespace Irc.Objects.Server;

public class Server : ChatObject, IServer
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    protected readonly IDataStore _dataStore;
    private readonly IFloodProtectionManager _floodProtectionManager;
    private readonly Task _processingTask;
    private readonly ISecurityManager _securityManager;
    private readonly ISocketServer _socketServer;
    private readonly IUserFactory _userFactory;
    public readonly ICommandCollection Commands;
    public IDictionary<EnumProtocolType, IProtocol> _protocols = new Dictionary<EnumProtocolType, IProtocol>();
    public IList<IChannel> Channels;
    private readonly ConcurrentQueue<IUser> PendingNewUserQueue = new();
    private readonly ConcurrentQueue<IUser> PendingRemoveUserQueue = new();

    public IList<IUser> Users = new List<IUser>();


    public Server(ISocketServer socketServer,
        ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager,
        IDataStore dataStore,
        IList<IChannel> channels,
        ICommandCollection commands,
        IUserFactory userFactory) : base(new ModeCollection(), dataStore)
    {
        _socketServer = socketServer;
        _securityManager = securityManager;
        _floodProtectionManager = floodProtectionManager;
        _dataStore = dataStore;
        Channels = channels;
        Commands = commands;
        _userFactory = userFactory;
        _processingTask = new Task(Process);
        _processingTask.Start();

        _dataStore.SetAs("creation", DateTime.UtcNow);
        _dataStore.Set("supported.channel.modes",
            new ChannelModes().GetSupportedModes());
        _dataStore.Set("supported.user.modes", new UserModes().GetSupportedModes());

        _protocols.Add(EnumProtocolType.IRC, new Irc());

        socketServer.OnClientConnecting += (sender, connection) =>
        {
            // TODO: Need to pass a Interfaced factory in to create the appropriate user
            // TODO: Need to start a new user out with protocol, below code is unreliable
            var user = CreateUser(connection);
            AddUser(user);

            connection.OnConnect += (o, integer) => { Console.WriteLine("Connect"); };
            connection.OnReceive += (o, s) =>
            {
                //Console.WriteLine("OnRecv:" + s);
            };
            connection.OnDisconnect += (o, integer) => RemoveUser(user);
            connection.Accept();
        };
        socketServer.Listen();
    }

    public DateTime CreationDate => _dataStore.GetAs<DateTime>("creation");

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

    public void AddUser(IUser user)
    {
        PendingNewUserQueue.Enqueue(user);
    }

    public void RemoveUser(IUser user)
    {
        PendingRemoveUserQueue.Enqueue(user);
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
        return new Channel.Channel(name, new ChannelModes(), new DataStore(name, "store"));
    }

    public IUser CreateUser(IConnection connection)
    {
        return _userFactory.Create(this, connection);
    }

    public IList<IUser> GetUsers()
    {
        return Users;
    }

    public IUser GetUserByNickname(string nickname) => Users.FirstOrDefault(user => string.Compare(user.GetAddress().Nickname.Trim(), nickname, true) == 0);

    public IList<IUser> GetUsersByList(string nicknames, char separator)
    {
        var list = nicknames.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();

        return GetUsersByList(list, separator);
    }

    public IList<IUser> GetUsersByList(List<string> nicknames, char separator)
    {
        return Users.Where(user => nicknames.Contains(user.GetAddress().Nickname, StringComparer.InvariantCultureIgnoreCase)).ToList();
    }

    public IList<IChannel> GetChannels()
    {
        return Channels;
    }

    public string GetSupportedChannelModes()
    {
        return _dataStore.Get("supported.channel.modes");
    }

    public string GetSupportedUserModes()
    {
        return _dataStore.Get("supported.user.modes");
    }

    public IDictionary<EnumProtocolType, IProtocol> GetProtocols()
    {
        return _protocols;
    }

    public System.Version GetVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version;
    }

    public IDataStore GetDataStore()
    {
        return _dataStore;
    }

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

    public override string ToString()
    {
        return Name;
    }

    private void Process()
    {
        var backoffMS = 0;
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var hasWork = false;

            RemovePendingUsers();
            AddPendingUsers();

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

    private void AddPendingUsers()
    {
        if (PendingNewUserQueue.Count > 0)
        {
            // add new pending users
            foreach (var user in PendingNewUserQueue) Users.Add(user);

            Console.WriteLine($"Added {PendingNewUserQueue.Count} users. Total Users = {Users.Count}");
            PendingNewUserQueue.Clear();
        }
    }

    private void RemovePendingUsers()
    {
        if (PendingRemoveUserQueue.Count > 0)
        {
            // remove pending to be removed users
            foreach (var user in PendingRemoveUserQueue) Users.Remove(user);

            Console.WriteLine($"Removed {PendingRemoveUserQueue.Count} users. Total Users = {Users.Count}");
            PendingRemoveUserQueue.Clear();
        }
    }

    protected void AddCommand(ICommand command, EnumProtocolType fromProtocol = EnumProtocolType.IRC, string name = null)
    {
        foreach (var protocol in _protocols)
        {
            if (protocol.Key >= fromProtocol) protocol.Value.AddCommand(command, name);
        }
    }

    private void ProcessNextCommand(IUser user)
    {
        var message = user.GetDataRegulator().PeekIncoming();
        Console.WriteLine($"Processing: " + message.OriginalText);
        var command = message.GetCommand();
        if (command != null)
        {
            var floodResult = _floodProtectionManager.Audit(user.GetFloodProtectionProfile(),
                command.GetDataType(), user.GetLevel());
            if (floodResult == EnumFloodResult.Ok)
            {
                user.GetDataRegulator().PopIncoming();
                //Console.WriteLine($"Processing: {message.OriginalText}");

                var chatFrame = new ChatFrame(this, user, message);
                if (command.CheckParameters(chatFrame))
                {
                    try
                    {
                        command.Execute(chatFrame);
                    }
                    catch (Exception e)
                    {
                        chatFrame.User.Send(IrcRaws.IRC_RAW_999(chatFrame.Server, chatFrame.User, e));
                    }
                }
                else user.Send(Raw.IRCX_ERR_NEEDMOREPARAMS_461(this, user, command.GetName()));

                // Check if user can register
                if (!chatFrame.User.IsRegistered()) Register.Execute(chatFrame);
            }
        }
        else
        {
            user.GetDataRegulator().PopIncoming();
            user.Send(Raw.IRCX_ERR_UNKNOWNCOMMAND_421(this, user, message.GetCommandName()));
            // command not found
        }
    }
}