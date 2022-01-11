using System;
using System.Collections.Generic;
using Core.CSharpTools;

namespace Core.Ircx.Objects;

public class Server : Client
{
    //Prop
    //Servers
    //Channels
    //Users
    //Modes
    //Access

    public Access Access;
    public ChannelCollection Channels = new();
    public CommandCollection Commands = new();

    public string CreationDate, TimeZone;

    // Statistics
    private int iMaxUsers;
    public ServerCollection Servers = new();
    public UserCollection Users = new();

    public Server(string Name) : base(ObjType.ServerObject)
    {
        Properties = new ServerProperties(this);
        this.Name = Name;
        Access = new Access("$", false);
        CreationDate = new string(Resources.GetFullTimeString(Resources.GetTime()));
        var TZ = DateTime.Now.GetDateTimeFormats('R')[0];
        TimeZone = new string(TZ.Substring(TZ.LastIndexOf(' ') + 1));
    }

    //Counting MaxUsers as total concurrent user objects (sockets)
    public int MaxUsers => Users.Length;
    public int RegisteredUsers { get; private set; }

    public int InvisibleCount { get; private set; }

    public int UnknownConnections { get; private set; }

    public int OperatorCount { get; }

    public int IrcxVersion { get; }

    public string FullName { get; set; }

    // Operations
    public Obj AddObject(string Name, ObjType objType, string ObjID)
    {
        Obj obj = null;
        switch (objType)
        {
            case ObjType.ServerObject:
            {
                var server = new Server(Name);
                Servers.Add(server);
                obj = server;
                break;
                //return server;
            }
            case ObjType.UserObject:
            {
                var user = new User();
                Users.Add(user);
                obj = user;
                break;
                //return user;
            }
            case ObjType.ChannelObject:
            {
                var channel = new Channel(Name);
                Channels.Add(channel);
                obj = channel;
                break;
                //return channel;
            }
            default:
            {
                return null;
            }
        }

        if (obj != null)
            if (ObjID != null)
                obj.SetForeignOID(ObjID);
        return obj;
    }

    public void RemoveObject(Obj Object)
    {
        if (Object != null)
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

    public User AddUser()
    {
        UnknownConnections++;

        // Check IP here
        var user = new User();
        Users.Add(user);

        return user;
    }

    public void RemUser(User user)
    {
        if (user.Registered)
        {
            user.Unregister();
            if (user.Modes.Invisible.Value == 1)
            {
                InvisibleCount--;
                user.Modes.Invisible.Value = 0;
            }

            RegisteredUsers--;
        }

        Users.Remove(user);
    }

    public Server AddServer()
    {
        var server = new Server(Resources.Wildcard);
        Servers.Add(server);
        return server;
    }

    public void RegisterUser(User user)
    {
        user.Register();
        UnknownConnections--;
        RegisteredUsers++;
    }

    public void InvisibleStatus(User user, bool IsInvisible)
    {
        if (IsInvisible && user.Modes.Invisible.Value == 0)
            InvisibleCount++;
        else if (!IsInvisible && user.Modes.Invisible.Value == 1) InvisibleCount--;
    }

    public Channel AddChannel(string Name)
    {
        var channel = new Channel(Name);
        Channels.Add(channel);
        return channel;
    }

    public void RemChannel(Channel channel)
    {
        Channels.Remove(channel);
    }

    public List<Obj> GetObjects(string Nicknames)
    {
        var objs = new List<Obj>();
        var NicknameList = Tools.CSVToArray(Nicknames);

        for (var i = 0; i < NicknameList.Count; i++)
        {
            var obj = GetObject(NicknameList[i]);
            if (obj != null) objs.Add(obj);
        }

        return objs;
    }

    public Obj GetObject(string Name)
    {
        Obj obj = null;
        var objIdentifier = IdentifyObject(Name);
        var objType = GetObjectType(Name, objIdentifier);
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
                        // Next User Priority
                        obj = Users.FindObjByOID(Name);
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


    public void UpdateUserNickname(User user, string Nickname)
    {
        // if OK
        user.Address.Nickname = Nickname;
        user.Access.ObjectName = Nickname;
        user.Properties.GetPropByName("NAME").Value = Nickname;
        user.Name = Nickname;
    }
}

public class ServerProperties : PropCollection
{
    public ServerProperties(Client obj) : base(obj)
    {
    }
}

public class ServerCollection : ObjCollection
{
    public ServerCollection() : base(ObjType.ServerObject)
    {
    }
}