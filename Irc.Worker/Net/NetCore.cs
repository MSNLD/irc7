using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Irc.ClassExtensions.CSharpTools;
using Irc.Helpers;
using Irc.Helpers.CSharpTools;

namespace Irc.Worker.Net;

public static class IPInfo
{
    public static IPAddress[] ips;

    public static async Task<IPAddress[]> GetAddresses()
    {
        var Host = default(IPHostEntry);
        string Hostname = null;
        Hostname = Environment.MachineName;
        Host = await Dns.GetHostEntryAsync(Hostname);
        ips = Host.AddressList;
        return Host.AddressList;
    }

    public static void PrintArray()
    {
        Console.WriteLine("Printing bindable local IPs");
        for (var c = 0; c < ips.Length; c++) Console.WriteLine("[" + c + "] " + ips[c]);
        Console.WriteLine("End of local IPs");
    }
}

public class CSocketInfo
{
    public int Count;
    public IPAddress IpAddress;
}

public class CSocketListener
{
    public int buffSize;
    public ConcurrentDictionary<Guid, long> ClientMap = new();
    public int maxClientsPerIP;
    public Socket Server;

    public CSocketListener(AddressFamily addressFamily, int buffSize, int MaxClientsPerIP)
    {
        Server = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        this.buffSize = buffSize;
        maxClientsPerIP = MaxClientsPerIP;
    }

    public CSocketListener(bool ipv6)
    {
        Server = new Socket(ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork, SocketType.Stream,
            ProtocolType.Tcp);
    }

    public void Listen(string bindip, int port, int backlog)
    {
        Listen(IPAddress.Parse(bindip), port, backlog);
    }

    public SocketException Listen(IPAddress bindIp, int port, int backlog)
    {
        //The below isnt supported in .NET Core yet, but may help get information regarding the connection before accepting
        //Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.PacketInformation, true);
        
        try
        {
            Server.Bind(new IPEndPoint(bindIp, port));
        }
        catch (SocketException se)
        {
            Debug.Out(se.Message);
            return se;
        }

        try
        {
            Server.Listen(backlog);
        }
        catch (SocketException se)
        {
            Debug.Out(se.Message);
            return se;
        }

        Debug.Out(string.Format("Listening on {0}:{1} backlog={2}", bindIp, port, backlog));

        return null;
    }

    public List<CSocket> Accept()
    {
        var AcceptClients = new List<CSocket>();

        // Need a break in the below while condition for flood prot
        // Changing this to IF to not block up the server if being spammed by sockets
        if (Server.Poll(0, SelectMode.SelectRead))
        {
            var socket = new CSocket(Server.Accept(), buffSize);

            // Check Count
            var count = ClientMap.GetOrAdd(socket.Address, 0);

            if (count <= maxClientsPerIP)
            {
                AcceptClients.Add(socket);
                Debug.Out("[" + AcceptClients.Count + "]connected " + socket.RemoteEndPoint);
                Debug.Out("[" + AcceptClients.Count + "]socket " +
                          socket.Address.ToUnformattedString() + " Count = " + count);
            }
            else
            {
                Debug.Out("[" + AcceptClients.Count + "]dumped socket " +
                          socket.Address.ToUnformattedString() + " Count = " + count);
                socket.Shutdown(SocketShutdown.Both);
                ClientMap.TryRemove(socket.Address, out _);
            }
        }

        return AcceptClients;
    }
}

public class CSocket
{
    private bool bTerminateSocket;
    private readonly StringBuffer buffer;
    private readonly Socket socket;

    public CSocket(Socket socket, int buffSize)
    {
        this.socket = socket;
        this.socket.Blocking = false;
        this.socket.LingerState = new LingerOption(true, 1);
        buffer = new StringBuffer(buffSize);

        byte[] addressBytes = ((IPEndPoint) socket.RemoteEndPoint).Address.GetAddressBytes();
        byte[] guidBytes = new byte[16];
        Array.Copy(addressBytes, 0, guidBytes, 0, addressBytes.Length);

        Address = new Guid(guidBytes);
    }

    public Guid Address { get; }

    public bool IsConnected => socket.Connected;
    public string RemoteIP => ((IPEndPoint) socket.RemoteEndPoint).Address.ToString();

    public EndPoint RemoteEndPoint => socket.RemoteEndPoint;

    public void Shutdown(SocketShutdown both)
    {
        Debug.Out(socket.RemoteEndPoint + " has been Shutdown (" + Address.ToUnformattedString() +
                  ")");
        try
        {
            socket.Shutdown(both);
        }
        catch (SocketException se)
        {
        }
    }

    public void Terminate()
    {
        bTerminateSocket = true;
    }

    public List<string> Process()
    {
        var bytes = -1;

        if (socket.Connected)
        {
            if (socket.Poll(0, SelectMode.SelectRead))
            {
                var b = new byte[buffer.Capacity];

                try
                {
                    bytes = socket.Receive(b);
                }
                catch (SocketException se)
                {
                }

                if (bytes > 0)
                {
                    buffer.Digest(b, bytes);
                    return buffer.DataIn;
                }

                Shutdown(SocketShutdown.Both);
            }

            if (socket.Poll(0, SelectMode.SelectWrite))
                if (buffer.bytesOut.Count > 0)
                {
                    try
                    {
                        bytes = socket.Send(buffer.bytesOut);
                    }
                    catch (SocketException se)
                    {
                    }

                    buffer.bytesOut.Clear();

                    if (bTerminateSocket) Shutdown(SocketShutdown.Both);
                }
        }
        else
        {
            Shutdown(SocketShutdown.Both);
        }

        return null;
    }

    public void Send(string data)
    {
        buffer.bytesOut.Add(new ArraySegment<byte>(data.ToByteArray(), 0, data.Length));
    }
}