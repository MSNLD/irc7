using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;
using Core.Ircx.Objects;
using Core.Ircx;

namespace Core.Ircx.Objects
{
    public class Server: Client
    {
        //Prop
        //Servers
        //Channels
        //Users
        //Modes
        //Access

        public Access Access;
        public UserCollection Users = new UserCollection();
        public ChannelCollection Channels = new ChannelCollection();
        public ServerCollection Servers = new ServerCollection();
        public CommandCollection Commands = new CommandCollection();

        public String8 CreationDate, TimeZone;

        public Server(String8 Name): base(ObjType.ServerObject)
        {
            Properties = new ServerProperties(this);
            base.Name = Name;
            Access = new Access("$", false);
            CreationDate = new String8(Resources.GetFullTimeString(Resources.GetTime()));
            string TZ = DateTime.Now.GetDateTimeFormats('R')[0];
            TimeZone = new String8(TZ.Substring(TZ.LastIndexOf(' ') + 1));
        }

        // Statistics
        private int iMaxUsers, iRegisteredUsers, iInvisibleCount, iUnknownConnections, iOperatorCount;
        private int iIrcxVersion;

        //Counting MaxUsers as total concurrent user objects (sockets)
        public int MaxUsers { get { return Users.Length; } }
        public int RegisteredUsers { get { return iRegisteredUsers; } }
        public int InvisibleCount { get { return iInvisibleCount; } }
        public int UnknownConnections { get { return iUnknownConnections; } }
        public int OperatorCount { get { return iOperatorCount; } }
        public int IrcxVersion { get { return iIrcxVersion; } }

        // Operations
        public Obj AddObject(String8 Name, ObjType objType, String8 ObjID)
        {
            Obj obj = null;
            switch (objType)
            {
                case ObjType.ServerObject:
                    {
                        Server server = new Server(Name);
                        Servers.Add(server);
                        obj = server;
                        break;
                        //return server;
                    }
                case ObjType.UserObject:
                    {
                        User user = new User();
                        Users.Add(user);
                        obj = user;
                        break;
                        //return user;
                    }
                case ObjType.ChannelObject:
                    {
                        Channel channel = new Channel(Name);
                        Channels.Add(channel);
                        obj = channel;
                        break;
                        //return channel;
                    }
                default: { return null;  }
            }

            if (obj != null)
            {
                if (ObjID != null)
                {
                    obj.SetForeignOID(ObjID);
                }
            }
            return obj;
        }
        public void RemoveObject(Obj Object)
        {
            if (Object != null)
            {
                switch (Object.ObjectType)
                {
                    case ObjType.ServerObject:
                        {
                            Servers.Remove(Object);
                            break;
                        }
                    case ObjType.UserObject:
                        {
                            Users.Remove(Object);
                            break;
                        }
                    case ObjType.ChannelObject:
                        {
                            Channels.Remove(Object);
                            break;
                        }
                }
            }
        }

        public User AddUser()
        {
            iUnknownConnections++;

            // Check IP here
            User user = new User();
            Users.Add(user);

            return user;
        }
        public void RemUser(User user)
        {
            if (user.Registered) {
                user.Unregister();
                if (user.Modes.Invisible.Value == 1) { iInvisibleCount--; user.Modes.Invisible.Value = 0; }
                iRegisteredUsers--;
            }
            Users.Remove(user);
        }
        public Server AddServer()
        {
            Server server = new Server(Resources.Wildcard);
            Servers.Add(server);
            return server;
        }
        public void RegisterUser(User user)
        {
            user.Register();
            iUnknownConnections--;
            iRegisteredUsers++;
        }
        public void InvisibleStatus(User user, bool IsInvisible)
        {
            if ((IsInvisible) && (user.Modes.Invisible.Value == 0)) { iInvisibleCount++; }
            else if ((!IsInvisible) && (user.Modes.Invisible.Value == 1)) { iInvisibleCount--; }
        }

        public Channel AddChannel(String8 Name)
        {
            Channel channel = new Channel(Name);
            Channels.Add(channel);
            return channel;
        }
        public void RemChannel(Channel channel)
        {
            Channels.Remove(channel);
        }

        public List<Obj> GetObjects(String8 Nicknames)
        {
            List<Obj> objs = new List<Obj>();
            List<String8> NicknameList = CSharpTools.Tools.CSVToArray(Nicknames);

            for (int i = 0; i < NicknameList.Count; i++)
            {
                Obj obj = GetObject(NicknameList[i]);
                if (obj != null) { objs.Add(obj); }
            }
            return objs;
        }

        public Obj GetObject(String8 Name)
        {
            Obj obj = null;
            ObjIdentifier objIdentifier = Client.IdentifyObject(Name);
            ObjType objType = Client.GetObjectType(Name, objIdentifier);
            switch (objType)
            {
                case ObjType.ObjectID:
                    {
                        // Find Object Recursively (Server highest priority)
                        obj = Servers.FindObjByOID(Name);
                        if (obj == null)
                        {
                            // Next Channel Priority
                            obj = Channels.FindObjByOID(Name);
                            if (obj == null)
                            {
                                // Next User Priority
                                obj = Users.FindObjByOID(Name);
                            }
                        }
                        break;
                    }
                case ObjType.ServerObject:
                    {
                        // In cases of $
                        obj = this;
                        break;
                    }
                case ObjType.ChannelObject:
                    {
                        obj = Channels.FindObj(Name, objIdentifier);
                        break;
                    }
                case ObjType.UserObject:
                    {
                        obj = Users.FindObj(Name, objIdentifier);
                        break;
                    }
            }
            return obj;
        }
       

        public void UpdateUserNickname(User user, String8 Nickname)
        {
            // if OK
            user.Address.Nickname = Nickname;
            user.Access.ObjectName = Nickname;
            user.Properties.GetPropByName("NAME").Value = Nickname;
            user.Name = Nickname;
        }
        
    }

    public class ServerProperties: PropCollection
    {
        public ServerProperties(Client obj): base(obj)
        {

        }
    }
    public class ServerCollection: ObjCollection
    {
        public ServerCollection(): base(ObjType.ServerObject)
        {

        }
    }
}
