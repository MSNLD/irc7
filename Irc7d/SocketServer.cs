using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using NLog;

namespace Irc7d;

public class SocketServer : Socket, ISocketServer
{
    public static readonly Logger Log = LogManager.GetCurrentClassLogger();

    public ConcurrentDictionary<BigInteger, ConcurrentBag<IConnection>> Sockets = new();


    public SocketServer(IPAddress ip, int port, int backlog, int maxConnectionsPerIP, int buffSize) : base(
        SocketType.Stream, ProtocolType.Tcp)
    {
        IP = ip;
        Port = port;
        Backlog = backlog;
        MaxConnectionsPerIp = maxConnectionsPerIP;
        BuffSize = buffSize;
        BuffSize = buffSize;
    }

    public IPAddress IP { get; }
    public EventHandler<IConnection> OnClientConnecting { get; set; }
    public EventHandler<IConnection> OnClientConnected { get; set; }
    public EventHandler<IConnection> OnClientDisconnected { get; set; }
    public EventHandler<ISocketServer> OnListen { get; set; }
    public int Port { get; }
    public int Backlog { get; }
    public int MaxConnectionsPerIp { get; }
    public int BuffSize { get; }
    public int CurrentConnections { get; }

    public void Listen()
    {
        Bind(new IPEndPoint(IP, Port));
        Listen(Backlog);

        // Callback for OnListen
        OnListen?.Invoke(this, this);

        var acceptAsync = new SocketAsyncEventArgs();
        acceptAsync.Completed += (sender, args) => { AcceptLoop(args); };

        // Get first socket
        AcceptAsync(acceptAsync);
    }

    public void Close()
    {
        Close();
    }


    public void AcceptConnection(Socket acceptSocket)
    {
        var connection = new SocketConnection(acceptSocket) as IConnection;
        OnClientConnecting?.Invoke(this, connection);
    }

    public void AcceptLoop(SocketAsyncEventArgs args)
    {
        do
        {
            AcceptConnection(args.AcceptSocket);
            // Get next socket
            // Reset AcceptSocket for next accept
            args.AcceptSocket = null;
        } while (!AcceptAsync(args));
    }

    public void Accept(IConnection connection)
    {
        if (Sockets.ContainsKey(connection.GetId()))
        {
            connection.Disconnect(
                "Too many connections"
            );
            return;
        }

        connection.OnDisconnect += ClientDisconnected;

        var socketCollection = Sockets.GetOrAdd(connection.GetId(), new ConcurrentBag<IConnection>());
        Log.Info($"Current keys: {Sockets.Count} / Current sockets: {socketCollection.Count}");

        socketCollection.Add(connection);
        connection.Accept();

        OnClientConnected?.Invoke(this, connection);
    }

    private void ClientDisconnected(object? sender, BigInteger bigIP)
    {
        IConnection connection = null;

        if (Sockets.ContainsKey(bigIP))
        {
            var bag = Sockets[bigIP];
            bag.TryTake(out connection);
        }

        if (connection == null)
            Log.Info(
                $"{connection.GetIpAndPort()} has disconnected but failed to TryTake / total: {Sockets.Count} ");

        OnClientDisconnected?.Invoke(this, connection);
    }
}