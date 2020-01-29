using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CSharpTools;

namespace Core.Net
{
    public static class IPInfo {
        public static IPAddress[] ips;
        public static async System.Threading.Tasks.Task<IPAddress[]> GetAddresses()
        {
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = await Dns.GetHostEntryAsync(Hostname);
            ips = Host.AddressList;
            return Host.AddressList;
        }
        public static void PrintArray()
        {
            Console.WriteLine("Printing bindable local IPs");
            for (int c = 0; c < ips.Length; c++)
            {
                Console.WriteLine("[" + c.ToString() + "] " + ips[c].ToString());
            }
            Console.WriteLine("End of local IPs");
        }
    }

    public class CSocketInfo {
        public GUID ipaddress;
        public int count;
    }


    public class CSocketListener {
        public Socket Server;
        public GuidMap ClientMap;
        public int buffSize;
        public int maxClientsPerIP;

        public CSocketListener(AddressFamily addressFamily, int buffSize, int MaxClientsPerIP)
        {
            Server = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.buffSize = buffSize;
            this.maxClientsPerIP = MaxClientsPerIP;
        }
        public CSocketListener(bool ipv6)
        {
            Server = new Socket((ipv6 ? AddressFamily.InterNetworkV6 : AddressFamily.InterNetwork), SocketType.Stream, ProtocolType.Tcp);
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

            try { 
                Server.Bind(new IPEndPoint(bindip, port));
            }
            catch (SocketException se) { Debug.Out(se.Message); return se; }

            try { 
                Server.Listen(backlog);
            }
            catch (SocketException se) { Debug.Out(se.Message); return se; }

            Debug.Out(String.Format("Listening on {0}:{1} backlog={2}", bindip.ToString(), port, backlog));

            return null;
        }
        public List<CSocket> Accept()
        {
            List<CSocket> AcceptClients = new List<CSocket>();

            // Need a break in the below while condition for flood prot
            // Changing this to IF to not block up the server if being spammed by sockets
            if (Server.Poll(0, SelectMode.SelectRead))
            {
                //EndPoint ep1 = Server.RemoteEndPoint;
                CSocket socket = new CSocket(Server.Accept(), buffSize);
                //EndPoint ep2 = socket.RemoteEndPoint;

                if (socket != null)
                {
                    // Check count
                    GuidNode node = ClientMap.AddGuid(socket.Address);
                    
                    if (node.count <= maxClientsPerIP)
                    {
                        AcceptClients.Add(socket);
                        Debug.Out("[" + AcceptClients.Count.ToString() + "]connected " + socket.RemoteEndPoint);
                        Debug.Out("[" + AcceptClients.Count.ToString() + "]socket " + new String8(node.guid.ToHex()).chars + " count = " + node.count.ToString());
                    }
                    else
                    {
                        Debug.Out("[" + AcceptClients.Count.ToString() + "]dumped socket " + new String8(node.guid.ToHex()).chars + " count = " + node.count.ToString());
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
        private Socket socket;
        private StringBuffer buffer;
        private GUID guid;
        private bool bTerminateSocket;

        public GUID Address { get { return guid; } }
        public bool IsConnected { get { return socket.Connected; } }
        public String8 RemoteIP { get { return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString(); } }

        public CSocket(Socket socket, int buffSize)
        {
            this.socket = socket;
            this.socket.Blocking = false;
            this.socket.LingerState = new LingerOption(true, 1);
            buffer = new StringBuffer(buffSize);
            guid = new GUID();
            if (socket.RemoteEndPoint.AddressFamily == AddressFamily.InterNetwork) { 
                guid.Data1 = BitConverter.ToUInt32(((IPEndPoint)socket.RemoteEndPoint).Address.GetAddressBytes(), 0);   
            }
            else
            {
                guid.SetBytes(((IPEndPoint)socket.RemoteEndPoint).Address.GetAddressBytes());
            }
        }

        public EndPoint RemoteEndPoint { get { return socket.RemoteEndPoint; } }
        public void Shutdown(SocketShutdown both)
        {
            Debug.Out(socket.RemoteEndPoint.ToString() + " has been Shutdown (" + (new String8(guid.ToHex()).chars + ")"));
            try { 
                socket.Shutdown(both);
            }
            catch (SocketException se) {  }

        }
        public void Terminate() { bTerminateSocket = true; }
        public List<String8> Process()
        {
            int bytes = -1;

            if (socket.Connected)
            {
                if (socket.Poll(0, SelectMode.SelectRead))
                {
                    byte[] b = new byte[buffer.Capacity];

                    try { bytes = socket.Receive(b); }
                    catch (SocketException se) { }

                    if (bytes > 0)
                    {
                        buffer.Digest(b, bytes);
                        return buffer.DataIn;
                    }
                    else { Shutdown(SocketShutdown.Both); }
                }
                if (socket.Poll(0, SelectMode.SelectWrite))
                {
                    if (buffer.bytesOut.Count > 0)
                    {
                        try {
                            bytes = socket.Send(buffer.bytesOut);
                        }
                        catch (SocketException se) { }
                        buffer.bytesOut.Clear();

                        if (bTerminateSocket) { Shutdown(SocketShutdown.Both); }
                    }
                }
            }
            else
            {
                Shutdown(SocketShutdown.Both);
            }
            return null;
        }
        public void Send(String8 data)
        {
            buffer.bytesOut.Add(new ArraySegment<byte>(data.bytes, 0, data.length));
        }
    }

}
