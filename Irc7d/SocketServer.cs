using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace Irc7d;

public class SocketServer: Socket, ISocketServer
{
    public ConcurrentDictionary<BigInteger, ConcurrentBag<IConnection>> Sockets = new();
    public EventHandler<IConnection> OnClientConnecting { get; set; }
    public EventHandler<IConnection> OnClientConnected { get; set; }
    public EventHandler<IConnection> OnClientDisconnected { get; set; }
    public EventHandler<ISocketServer> OnListen { get; set; }

    public IPAddress IP { get; }
    public int Port { get; }
    public int Backlog { get; }
    public int MaxConnectionsPerIp { get; }
    public int BuffSize { get; }
    public int CurrentConnections { get; }


    public SocketServer(IPAddress ip, int port, int backlog, int maxConnectionsPerIP, int buffSize) : base(SocketType.Stream, ProtocolType.Tcp)
    {
        IP = ip;
        Port = port;
        Backlog = backlog;
        MaxConnectionsPerIp = maxConnectionsPerIP;
        BuffSize = buffSize;
        BuffSize = buffSize;
    }

    public void Listen()
    {
        Bind(new IPEndPoint(IP, Port));
        Listen(Backlog);

        // Callback for OnListen
        OnListen?.Invoke(this, this);

        var acceptAsync = new SocketAsyncEventArgs();
        acceptAsync.Completed += (sender, args) =>
        {
            var connection = new SocketConnection(args.AcceptSocket) as IConnection;
            OnClientConnecting?.Invoke(this, connection);

            // Get next socket
            // Reset AcceptSocket for next accept
            args.AcceptSocket = null;
            AcceptAsync(acceptAsync);
        };

        // Get first socket
        AcceptAsync(acceptAsync);
    }

    public void Accept(IConnection connection)
    {
        connection.OnDisconnect += ClientDisconnected;

        var socketCollection = Sockets.GetOrAdd(connection.GetId(), new ConcurrentBag<IConnection>());
        Console.WriteLine($"Current keys: {Sockets.Count} / Current sockets: {socketCollection.Count}");

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
            Console.WriteLine($"{connection.GetFullAddress()} has disconnected but failed to TryTake / total: {Sockets.Count} ");

        OnClientDisconnected?.Invoke(this, connection);
    }

    public void Close()
    {
        Close();
    }
}