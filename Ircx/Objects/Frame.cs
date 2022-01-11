using System.Collections.Generic;

namespace Core.Ircx.Objects;

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
        if (Client.ObjectType == ObjType.ServerObject)
        {
            this.Server = (Server) Client;

            if (Message != null)
                if (Message.Prefix != null)
                {
                    var obj = Server.GetObject(Message.Prefix);
                    if (obj != null)
                    {
                        if (obj.ObjectType == ObjType.UserObject) User = (User) obj;
                    }
                    else
                    {
                        obj = Server.AddObject(Message.Prefix, ObjType.UserObject, Message.Prefix);
                    }
                }
        }
        else
        {
            this.Server = Server;
            User = (User) Client;
        }

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