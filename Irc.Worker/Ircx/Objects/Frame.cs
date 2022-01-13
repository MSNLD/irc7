using System.Collections.Generic;

namespace Irc.Worker.Ircx.Objects;

public class Frame
{
    public Channel Channel;
    public Client Client;

    public Command Command;
    public Connection Connection;
    public Message Message;
    public string Reply;
    public Server Server;
    public List<User> TargetUser;
    public User User;

    public Frame(Server Server, Connection Connection, Message Message)
    {
        this.Connection = Connection;
        var Client = Connection.Client;
        this.Client = Client;
        this.Server = Server;
        User = (User) Client;

        if (Message != null)
        {
            Command = (Command) Server.Commands.GetCommand(Message.Command);
            this.Message = Message;
        }
    }
}

public static class DefaultFrames
{
    public static Frame NotImplemented = new(null, null, new Message("Not Implemented"));
}