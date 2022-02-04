using System;
using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;
using Irc.Commands;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Protocols;
using Irc.Security;
using Irc7d;
using User = Irc.Objects.User;

namespace Irc.Worker.Ircx.Objects;

public class Server: ChatItem
{
    // Server Properties To be moved to another class later
    public bool AnnonymousAllowed { get; }
    public int ChannelCount { get; }
    public IList<ChatItem> IgnoredUsers { get; }
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
    public int CreationDate;
    public string RemoteIP { set; get; }


    public IList<User> Users = new List<User>();
    private readonly ISocketServer _socketServer;
    private readonly ISecurityManager _securityManager;
    private readonly IFloodProtectionManager _floodProtectionManager;
    public IList<IChannel> Channels;
    public readonly ICommandCollection Commands;
    public IDictionary<EnumProtocolType, IProtocol> _protocols = new Dictionary<EnumProtocolType, IProtocol>();
    private Task _processingTask;
    private CancellationTokenSource _cancellationTokenSource = new ();

    public Server(ISocketServer socketServer, ISecurityManager securityManager, IFloodProtectionManager floodProtectionManager, IObjectStore objectStore, IPropStore propStore, IList<IChannel> channels, ICommandCollection commands): base(objectStore, propStore)
    {
        Name = "TK2CHATCHATA01";
        _socketServer = socketServer;
        _securityManager = securityManager;
        _floodProtectionManager = floodProtectionManager;
        Channels = channels;
        Commands = commands;
        Name = Name;
        _processingTask = new Task(Process);
        _processingTask.Start();

        _protocols.Add(EnumProtocolType.IRC, new Irc());
        _protocols.Add(EnumProtocolType.IRCX, new IrcX());

        socketServer.OnClientConnecting += (sender, connection) =>
        {
            // TODO: Need to start a new user out with protocol, below code is unreliable
            User user = new User(connection, _protocols.First().Value, new DataRegulator(MaxInputBytes, MaxOutputBytes), new FloodProtectionProfile(), new ObjectStore(), new PropCollection(), null, this);
            AddUser(user);
            user.Address.RemoteIP = connection.GetAddress();

            connection.OnConnect += (o, integer) =>
            {
                Console.WriteLine("Connect");
            };
            connection.OnReceive += (o, s) =>
            {
                //Console.WriteLine("OnRecv:" + s);

            };
            connection.Accept();
        };
        socketServer.Listen();
    }

    private void Process()
    {
        int backoffMS = 0;
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var hasWork = false;

            // do stuff
            foreach (User user in Users)
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

    public IList<User> GetUsers() => Users;
    public IList<IChannel> GetChannels() => Channels;

    public IChannel GetChannelByName(string name) => Channels.SingleOrDefault(c =>
        string.Equals(c.GetName(), name, StringComparison.InvariantCultureIgnoreCase));

    public IProtocol GetProtocol(EnumProtocolType protocolType)
    {
        if (_protocols.TryGetValue(protocolType, out var protocol)) return protocol;
        return null;
    }

    public ISecurityManager GetSecurityManager() => _securityManager;
    public ICredentialProvider GetCredentialManager() => null;
}