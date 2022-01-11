using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core.Ircx;
using Core.Ircx.Commands;
using Core.Ircx.Objects;
using Core.Ircx.Runtime;
using Core.Net;
using CSharpTools;

namespace Core;

public class Connection
{
    public Client Client;
    public CSocket Socket;

    public Connection(CSocket ConnectingSocket, Client ConnectingUser)
    {
        Socket = ConnectingSocket;
        Client = ConnectingUser;
        Client.Address.RemoteIP = Socket.RemoteIP;
    }
}

public class Engine
{
    private readonly List<Connection> ClientConnections = new();
    private readonly CSocketListener Listener;
    private readonly Server Server;

    public Engine(Server BaseServer, IPAddress listeningAddress, int buffSize, int maxClientsPerIP)
    {
        Server = BaseServer;
        Listener = new CSocketListener(listeningAddress.AddressFamily, buffSize, maxClientsPerIP);
        Server = BaseServer;
    }

    public void ProcessData(Connection Connection)
    {
        var Client = Connection.Client;
        if (Client.InputQueue.Count > 0)
        {
            var Frame = Client.InputQueue.Peek();
            var DataType = CommandDataType.None;
            if (Frame.Command != null) DataType = Frame.Command.DataType;

            if (Frame.User != null)
                if (Frame.User.Level >= UserAccessLevel.ChatGuide)
                    DataType = CommandDataType.HostMessage;

            var Saturation = Frame.Client.FloodProfile.Saturation;

            if (Saturation != SAT_RESULT.S_OK)
            {
                Client.Send(Raws.Create(Frame.Server, Client: Client,
                    Raw: Saturation == SAT_RESULT.S_INPUT_EXCEEDED
                        ? Raws.IRCX_CLOSINGLINK_008_INPUTFLOODING
                        : Raws.IRCX_CLOSINGLINK_009_OUTPUTSATURATION,
                    Data: new[] {Frame.Client.Address.RemoteIP}));
                Client.InputQueue.Clear();
                QUIT.ProcessQuit(Frame.Server, Frame.Client, Resources.INPUTFLOODING);
                Client.FloodProfile.currentInputBytes = 0;
            }

            try
            {
                if (Client.Process(Frame) != COM_RESULT.COM_WAIT)
                {
                    Client.InputQueue.Dequeue();
                    if (Client.FloodProfile.currentInputBytes > 0)
                        Client.FloodProfile.currentInputBytes -= (uint) Frame.Message.rawData.Length;
                }
            }
            catch (Exception e)
            {
                //lets get rid of the thing that caused the error and report it
                if (Client.InputQueue.Count > 0) Client.InputQueue.Dequeue();
                if (Client.FloodProfile.currentInputBytes > 0)
                    Client.FloodProfile.currentInputBytes -= (uint) Frame.Message.rawData.Length;
                var Mask = Resources.Wildcard;
                if (Client.Address._address[3] != null) Mask = Client.Address._address[3];
                Debug.Out(Client.Address.RemoteIP + " (" + Mask + ")\r\n =>" + Frame.Message.rawData);
                Debug.Out(e.Message);
                Client.Send(Raws.Create(Frame.Server, Client: Client, Raw: Raws.IRCX_ERR_EXCEPTION,
                    Data: new[] {Frame.Message.Data[0]}));
            }
        }

        if (!Client.Registered) Register.QualifyUser(Server, Connection);
    }

    public void Start(IPAddress listeningAddress, int port, int buffSize, int backLog)
    {
        var LastInterval = DateTime.UtcNow;
        var bSecondInterval = false;
        var iExportSecondCounter = 0;
        var se = Listener.Listen(listeningAddress, port, backLog);

        if (se == null)
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
                var Connections = Listener.Accept();

                for (var c = 0; c < Connections.Count; c++)
                {
                    var user = Server.AddUser();
                    if (user != null)
                    {
                        var newClient = new Connection(Connections[c], user);
                        ClientConnections.Add(newClient);
                    }
                    else
                    {
                        Connections[c].Shutdown(SocketShutdown.Both);
                        Listener.ClientMap.DecGuid(Connections[c].Address);
                    }
                }

                // Data Block
                for (var c = 0; c < ClientConnections.Count; c++)
                    if (!ClientConnections[c].Socket.IsConnected)
                    {
                        if (ClientConnections[c].Client.IsConnected)
                        {
                            QUIT.ProcessQuit(Server, ClientConnections[c].Client, Resources.CONNRESETBYPEER);
                            ClientConnections[c].Client.Terminate();
                        }

                        var cli = ClientConnections[c];
                        Server.RemoveObject(cli.Client);
                        ClientConnections.Remove(cli);
                        Listener.ClientMap.DecGuid(cli.Socket.Address);
                    }
                    else if (!ClientConnections[c].Client.IsConnected &&
                             ClientConnections[c].Socket.IsConnected) // if sockets gone no need to clean up
                    {
                        //Send
                        var OutputData = ClientConnections[c].Client.BufferOut;
                        while (OutputData.Count > 0) ClientConnections[c].Socket.Send(OutputData.Dequeue());
                        ClientConnections[c].Socket.Process();
                        ClientConnections[c].Socket.Shutdown(SocketShutdown.Both);
                    }
                    else
                    {
                        //Recv
                        var InputData = ClientConnections[c].Socket.Process();
                        if (InputData != null)
                        {
                            for (var x = 0; x < InputData.Count; x++)
                            {
                                var message = new Message(InputData[x]);
                                if (message != null)
                                    if (message.Command != null)
                                    {
                                        var frame = new Frame(Server, ClientConnections[c], new Message(InputData[x]));
                                        ClientConnections[c].Client.Receive(frame);
                                        //Client[c].User.Enqueue(frame);
                                    }
                            }

                            InputData.Clear();
                        }
                        else
                        {
                            // Check ping
                            if (!ClientConnections[c].Client.WaitPing &&
                                (DateTime.UtcNow.Ticks - ClientConnections[c].Client.LastPing) /
                                TimeSpan.TicksPerSecond > Program.Config.PingTimeout)
                            {
                                if (Debug.Enabled)
                                    Debug.Out(string.Format(
                                        "Ping not received from {0} since {1}. WaitPing flag is {2}",
                                        ClientConnections[c].Client.Name,
                                        new DateTime(ClientConnections[c].Client.LastPing).ToString(),
                                        ClientConnections[c].Client.WaitPing));
                                ClientConnections[c].Client.LastPing = DateTime.UtcNow.Ticks;
                                ClientConnections[c].Client.Send(Raws.Create(Server,
                                    Client: ClientConnections[c].Client, Raw: Raws.RPL_PING));
                                ClientConnections[c].Client.WaitPing = true;
                            }
                            else if (ClientConnections[c].Client.WaitPing &&
                                     (DateTime.UtcNow.Ticks - ClientConnections[c].Client.LastPing) /
                                     TimeSpan.TicksPerSecond > Program.Config.PingTimeout)
                            {
                                if (Debug.Enabled)
                                    Debug.Out(string.Format(
                                        "Ping timeout: no ping received from {0} since {1}. WaitPing flag is {2}",
                                        ClientConnections[c].Client.Name,
                                        new DateTime(ClientConnections[c].Client.LastPing).ToString(),
                                        ClientConnections[c].Client.WaitPing));

                                // pingtimeout
                                if (ClientConnections[c].Client.Registered)
                                    ClientConnections[c].Client.Send(Raws.Create(Server,
                                        Client: ClientConnections[c].Client, Raw: Raws.IRCX_CLOSINGLINK_011_PINGTIMEOUT,
                                        Data: new[] {ClientConnections[c].Client.Address.RemoteIP}));
                                QUIT.ProcessQuit(Server, ClientConnections[c].Client, Resources.PINGTIMEOUT);
                            }
                        }


                        //Process Input
                        ProcessData(ClientConnections[c]);

                        //Send
                        var OutputData = ClientConnections[c].Client.BufferOut;
                        while (OutputData.Count > 0) ClientConnections[c].Socket.Send(OutputData.Dequeue());
                    }

                if (bSecondInterval) iExportSecondCounter++;

                if (iExportSecondCounter >= 300)
                {
                    Server.Channels.RemoveEmptyChannels();
                    try
                    {
                        Stats.ExportCategories(Server);
                        Stats.ExportUserList(Server);
                    }
                    catch (Exception e)
                    {
                    }

                    iExportSecondCounter = 0;
                }

                Thread.Sleep(1);
            }
    }

    public void Stop()
    {
        //cleanup
        Listener.Server.Shutdown(SocketShutdown.Both);
    }
}