using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Core.CSharpTools;
using CSharpTools;

namespace Core.Net;

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
    public int count;
    public GUID ipaddress;
}

public class CSocketListener
{
    public int buffSize;
    public GuidMap ClientMap;
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

    public SocketException Listen(IPAddress bindip, int port, int backlog)
    {
        //The below isnt supported in .NET Core yet, but may help get information regarding the connection before accepting
        //Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.PacketInformation, true);

        ClientMap = new GuidMap();

        try
        {
            Server.Bind(new IPEndPoint(bindip, port));
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

        Debug.Out(string.Format("Listening on {0}:{1} backlog={2}", bindip, port, backlog));

        return null;
    }

    public List<CSocket> Accept()
    {
        var AcceptClients = new List<CSocket>();

        // Need a break in the below while condition for flood prot
        // Changing this to IF to not block up the server if being spammed by sockets
        if (Server.Poll(0, SelectMode.SelectRead))
        {
            //EndPoint ep1 = Server.RemoteEndPoint;
            var socket = new CSocket(Server.Accept(), buffSize);
            //EndPoint ep2 = socket.RemoteEndPoint;

            if (socket != null)
            {
                // Check count
                var node = ClientMap.AddGuid(socket.Address);

                if (node.count <= maxClientsPerIP)
                {
                    AcceptClients.Add(socket);
                    Debug.Out("[" + AcceptClients.Count + "]connected " + socket.RemoteEndPoint);
                    Debug.Out("[" + AcceptClients.Count + "]socket " +
                              StringBuilderExtensions.FromBytes(node.guid.ToHex()) + " count = " + node.count);
                }
                else
                {
                    Debug.Out("[" + AcceptClients.Count + "]dumped socket " +
                              StringBuilderExtensions.FromBytes(node.guid.ToHex()) + " count = " + node.count);
                    socket.Shutdown(SocketShutdown.Both);
                    ClientMap.DecGuid(socket.Address);
                }
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

        Address = new GUID();
        if (socket.RemoteEndPoint.AddressFamily == AddressFamily.InterNetwork)
            Address.Data1 = BitConverter.ToUInt32(((IPEndPoint) socket.RemoteEndPoint).Address.GetAddressBytes(), 0);
        else
            Address.SetBytes(((IPEndPoint) socket.RemoteEndPoint).Address.GetAddressBytes());
    }

    public GUID Address { get; }

    public bool IsConnected => socket.Connected;
    public string RemoteIP => ((IPEndPoint) socket.RemoteEndPoint).Address.ToString();

    public EndPoint RemoteEndPoint => socket.RemoteEndPoint;

    public void Shutdown(SocketShutdown both)
    {
        Debug.Out(socket.RemoteEndPoint + " has been Shutdown (" + StringBuilderExtensions.FromBytes(Address.ToHex()) +
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