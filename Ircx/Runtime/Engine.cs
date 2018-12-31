using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using CSharpTools;
using Core.Net;
using System.Net.Sockets;
using Core.Ircx.Objects;
using Core.Ircx;

namespace Core
{
    public class Connection
    {
        public CSocket Socket;
        public Client Client;

        public Connection(CSocket ConnectingSocket, Client ConnectingUser)
        {
            this.Socket = ConnectingSocket;
            this.Client = ConnectingUser;
            this.Client.Address.RemoteIP = Socket.RemoteIP;
        }
    }
    public class Engine
    {
        CSocketListener Listener;
        Server Server;
        List<Connection> ClientConnections = new List<Core.Connection>();

        public Engine(Server BaseServer, IPAddress listeningAddress, int buffSize, int maxClientsPerIP)
        {
            Server = BaseServer;
            Listener = new CSocketListener(listeningAddress.AddressFamily, buffSize, maxClientsPerIP);
            Server = BaseServer;
        }

        public void ProcessData(Connection Connection)
        {
            Client Client = (Client)Connection.Client;
            if (Client.InputQueue.Count > 0)
            {


                Frame Frame = Client.InputQueue.Peek();
                CommandDataType DataType = CommandDataType.None;
                if (Frame.Command != null) { DataType = Frame.Command.DataType; }

                if (Frame.User != null) { 
                    if (Frame.User.Level >= UserAccessLevel.ChatGuide) { DataType = CommandDataType.HostMessage; }
                }

                SAT_RESULT Saturation = Frame.Client.FloodProfile.Saturation;

                if (Saturation != SAT_RESULT.S_OK)
                {
                    Client.Send(Raws.Create(Server: Frame.Server, Client: Client,
                        Raw: (Saturation == SAT_RESULT.S_INPUT_EXCEEDED ? Raws.IRCX_CLOSINGLINK_008_INPUTFLOODING : Raws.IRCX_CLOSINGLINK_009_OUTPUTSATURATION),
                        Data: new String8[] { Frame.Client.Address.RemoteIP }));
                    Client.InputQueue.Clear();
                    Core.Ircx.Commands.QUIT.ProcessQuit(Frame.Server, Frame.Client, Resources.INPUTFLOODING);
                    Client.FloodProfile.currentInputBytes = 0;
                }

                try
                {
                    if (Client.Process(Frame) != COM_RESULT.COM_WAIT) { 
                        Client.InputQueue.Dequeue();
                        if (Client.FloodProfile.currentInputBytes > 0) { 
                            Client.FloodProfile.currentInputBytes -= (uint)Frame.Message.rawData.length;
                        }
                    }
                }
                catch (Exception e)
                {
                    //lets get rid of the thing that caused the error and report it
                    if (Client.InputQueue.Count > 0) { Client.InputQueue.Dequeue(); }
                    if (Client.FloodProfile.currentInputBytes > 0)
                    {
                        Client.FloodProfile.currentInputBytes -= (uint)Frame.Message.rawData.length;
                    }
                    String8 Mask = Resources.Wildcard;
                    if (Client.Address._address[3] != null) { Mask = Client.Address._address[3]; }
                    Debug.Out((Client.Address.RemoteIP.chars + " (" + Mask.chars + ")\r\n =>" + Frame.Message.rawData.chars));
                    Debug.Out(e.Message);
                    Client.Send(Raws.Create(Server: Frame.Server, Client: Client, Raw: Raws.IRCX_ERR_EXCEPTION, Data: new String8[] { Frame.Message.Data[0] }));
                }
            }
            if (!Client.Registered)
            {
                Core.Ircx.Runtime.Register.QualifyUser(Server, Connection);
            }
        }

        public void Start(IPAddress listeningAddress, int port, int buffSize, int backLog)
        {
            DateTime LastInterval = DateTime.UtcNow;
            bool bSecondInterval = false;
            int iExportSecondCounter = 0;
            SocketException se = Listener.Listen(listeningAddress, port, backLog);

            if (se == null)
            {
                for (;;)
                {
                    if ((DateTime.UtcNow - LastInterval).Seconds > 0)
                    {
                        bSecondInterval = true;
                        LastInterval = DateTime.UtcNow;
                    }
                    else
                    {
                        bSecondInterval = false; 
                    }

                    // Connecting Block
                    List<CSocket> Connections = Listener.Accept();

                    for (int c = 0; c < Connections.Count; c++)
                    {
                        User user = Server.AddUser();
                        if (user != null)
                        {
                            Connection newClient = new Core.Connection(Connections[c], user);
                            ClientConnections.Add(newClient);
                        }
                        else
                        {
                            Connections[c].Shutdown(SocketShutdown.Both);
                            Listener.ClientMap.DecGuid(Connections[c].Address);
                        }
                    }

                    // Data Block
                    for (int c = 0; c < ClientConnections.Count; c++)
                    {
                        if (!ClientConnections[c].Socket.IsConnected)
                        {
                            if (ClientConnections[c].Client.IsConnected)
                            {
                                Ircx.Commands.QUIT.ProcessQuit(Server, ClientConnections[c].Client, Resources.CONNRESETBYPEER);
                                ClientConnections[c].Client.Terminate();
                            }
                            Connection cli = ClientConnections[c];
                            Server.RemoveObject(cli.Client);
                            ClientConnections.Remove(cli);
                            Listener.ClientMap.DecGuid(cli.Socket.Address);
                        }
                        else if ((!ClientConnections[c].Client.IsConnected) && (ClientConnections[c].Socket.IsConnected)) // if sockets gone no need to clean up
                        {
                            //Send
                            Queue<String8> OutputData = ClientConnections[c].Client.BufferOut;
                            while (OutputData.Count > 0)
                            {
                                ClientConnections[c].Socket.Send(OutputData.Dequeue());
                            }
                            ClientConnections[c].Socket.Process();
                            ClientConnections[c].Socket.Shutdown(SocketShutdown.Both);
                        }
                        else { 
                            //Recv
                            List<String8> InputData = ClientConnections[c].Socket.Process();
                            if (InputData != null) { 
                                for (int x = 0; x < InputData.Count; x++)
                                {
                                    Message message = new Message(InputData[x]);
                                    if (message != null)
                                    {
                                        if (message.Command != null)
                                        {
                                            Frame frame = new Frame(Server, ClientConnections[c], new Ircx.Message(InputData[x]));
                                            ClientConnections[c].Client.Receive(frame);
                                            //Client[c].User.Enqueue(frame);
                                        }
                                    }
                                    else
                                    {
                                        // Blank data?
                                    }
                                }
                                InputData.Clear();
                            }
                            else
                            {
                                // Check ping
                                if ((!ClientConnections[c].Client.WaitPing) && ((DateTime.UtcNow.Ticks - ClientConnections[c].Client.LastPing) / TimeSpan.TicksPerSecond) > Program.Config.PingTimeout)
                                {
                                    if (Debug.Enabled) { Debug.Out(String.Format("Ping not received from {0} since {1}. WaitPing flag is {2}", ClientConnections[c].Client.Name.chars, new DateTime(ClientConnections[c].Client.LastPing).ToString(), ClientConnections[c].Client.WaitPing)); }
                                    ClientConnections[c].Client.LastPing = DateTime.UtcNow.Ticks;
                                    ClientConnections[c].Client.Send(Raws.Create(Server, Client: ClientConnections[c].Client, Raw: Raws.RPL_PING));
                                    ClientConnections[c].Client.WaitPing = true;
                                }
                                else if ((ClientConnections[c].Client.WaitPing) && ((DateTime.UtcNow.Ticks - ClientConnections[c].Client.LastPing) / TimeSpan.TicksPerSecond) > Program.Config.PingTimeout)
                                {
                                    if (Debug.Enabled) { Debug.Out(String.Format("Ping timeout: no ping received from {0} since {1}. WaitPing flag is {2}", ClientConnections[c].Client.Name.chars, new DateTime(ClientConnections[c].Client.LastPing).ToString(), ClientConnections[c].Client.WaitPing)); }

                                    // pingtimeout
                                    if (ClientConnections[c].Client.Registered) { ClientConnections[c].Client.Send(Raws.Create(Server: Server, Client: ClientConnections[c].Client, Raw: Raws.IRCX_CLOSINGLINK_011_PINGTIMEOUT, Data: new String8[] { ClientConnections[c].Client.Address.RemoteIP })); }
                                    Ircx.Commands.QUIT.ProcessQuit(Server, ClientConnections[c].Client, Resources.PINGTIMEOUT);
                                }
                            }


                            //Process Input
                            ProcessData(ClientConnections[c]);

                            //Send
                            Queue<String8> OutputData = ClientConnections[c].Client.BufferOut;
                            while (OutputData.Count > 0)
                            {
                                ClientConnections[c].Socket.Send(OutputData.Dequeue());
                            }
                        }
                    }

                    if (bSecondInterval) { iExportSecondCounter++; }

                    if (iExportSecondCounter >= 300)
                    {
                        Server.Channels.RemoveEmptyChannels();
                        try {
                            Core.Ircx.Runtime.Stats.ExportCategories(Server);
                            Core.Ircx.Runtime.Stats.ExportUserList(Server);
                        }
                        catch (Exception e) { }
                        iExportSecondCounter = 0;
                    }

                    System.Threading.Thread.Sleep(1);
                }
            }
            else
            {
                //there has been an error
            }
        }
        public void Stop()
        {
            //cleanup
            Listener.Server.Shutdown(SocketShutdown.Both);
        }
    }
}
