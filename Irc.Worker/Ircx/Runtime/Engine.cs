using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx;
using Irc.Worker.Ircx.Commands;
using Irc.Worker.Ircx.Objects;
using Irc.Worker.Ircx.Runtime;
using Irc.Worker.Net;

namespace Irc.Worker;

public class Connection
{
    public Client Client;
    public CSocket Socket;

    public Connection(CSocket ConnectingSocket, Client ConnectingUser)
    {
        Socket = ConnectingSocket;
        Client = ConnectingUser;
        Client.RemoteIP = Socket.RemoteIP;
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
                Client.Send(RawBuilder.Create(Frame.Server, Client: Client,
                    Raw: Saturation == SAT_RESULT.S_INPUT_EXCEEDED
                        ? Raws.IRCX_CLOSINGLINK_008_INPUTFLOODING
                        : Raws.IRCX_CLOSINGLINK_009_OUTPUTSATURATION,
                    Data: new[] {Frame.Client.RemoteIP}));
                Client.InputQueue.Clear();
                QUIT.ProcessQuit(Frame.Server, Frame.Client, Resources.INPUTFLOODING);
                Client.FloodProfile.currentInputBytes = 0;
            }

            try
            {
                if (Client.Process(Frame) != false)
                {
                    Client.InputQueue.Dequeue();
                    if (Client.FloodProfile.currentInputBytes > 0)
                        Client.FloodProfile.currentInputBytes -= (uint) Frame.Message.OriginalText.Length;
                }
            }
            catch (Exception e)
            {
                //lets get rid of the thing that caused the error and report it
                if (Client.InputQueue.Count > 0) Client.InputQueue.Dequeue();
                if (Client.FloodProfile.currentInputBytes > 0)
                    Client.FloodProfile.currentInputBytes -= (uint) Frame.Message.OriginalText.Length;
                var Mask = Resources.Wildcard;
                if (Client.Address.GetFullAddress() != null) Mask = Client.Address.GetFullAddress();
                Debug.Out(Client.RemoteIP + " (" + Mask + ")\r\n =>" + Frame.Message.OriginalText);
                Debug.Out(e.Message);
                Client.Send(RawBuilder.Create(Frame.Server, Client: Client, Raw: Raws.IRCX_ERR_EXCEPTION,
                    Data: new[] {Frame.Message.Parameters[0]}));
            }
        }

        if (!Client.Registered) Register.QualifyUser(Server, Connection);
    }

    private void RemoveEmptyChannels()
    {
        for (var i = Server.Channels.Count - 1; i >= 0; i--)
        {
            var c = (Channel)Server.Channels[i];
            if (c.Members.Count == 0 && c.Modes.Registered.Value != 0x1) Server.Channels.Remove(c);
        }
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
                        Listener.ClientMap.TryRemove(Connections[c].Address, out _);
                    }
                }

                // Parameters Block
                for (var c = 0; c < ClientConnections.Count; c++)
                    if (!ClientConnections[c].Socket.IsConnected)
                    {
                        if (ClientConnections[c].Client.IsConnected)
                        {
                            QUIT.ProcessQuit(Server, ClientConnections[c].Client, Resources.CONNRESETBYPEER);
                            ClientConnections[c].Client.Terminate();
                        }

                        var cli = ClientConnections[c];
                        Server.RemoveUser(cli.Client as User);
                        ClientConnections.Remove(cli);
                        Listener.ClientMap.TryRemove(cli.Socket.Address, out _);
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
                                    if (message.GetCommand() != null)
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
                                ClientConnections[c].Client.Send(RawBuilder.Create(Server,
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
                                    ClientConnections[c].Client.Send(RawBuilder.Create(Server,
                                        Client: ClientConnections[c].Client, Raw: Raws.IRCX_CLOSINGLINK_011_PINGTIMEOUT,
                                        Data: new[] {ClientConnections[c].Client.RemoteIP}));
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
                    RemoveEmptyChannels();
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