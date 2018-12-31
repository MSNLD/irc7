using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;


namespace Core.Ircx.Objects
{
    public class Frame
    {
        public Server Server;
        public Channel Channel;
        public User User;
        public List<User> TargetUser;
        public Client Client;
        public Connection Connection;

        public Command Command;
        public Message Message;
        public String8 Reply;

        public Frame(Server Server, Connection Connection, Message Message)
        {
            this.Connection = Connection;
            Client Client = (Client)Connection.Client;
            this.Client = Client;
            if (Client.ObjectType == ObjType.ServerObject)
            {
                this.Server = (Server)Client;

                if (Message != null)
                {
                    if (Message.Prefix != null) {
                        Obj obj = Server.GetObject(Message.Prefix);
                        if (obj != null) { 
                            if (obj.ObjectType == ObjType.UserObject)
                            {
                                this.User = (User)obj;
                            }
                        }
                        else
                        {
                            obj = Server.AddObject(Message.Prefix, ObjType.UserObject, Message.Prefix);
                        }
                    }
                }
            }
            else
            {
                this.Server = Server;
                this.User = (User)Client;
            }

            if (Message != null)
            {
                Command = (Command)Server.Commands.GetCommand(Message.Command);
                this.Message = Message;
            }
        }
    }

    static public class DefaultFrames {
        public static Frame NotImplemented = new Frame(null, null, new Message("Not Implemented"));
    }

}
