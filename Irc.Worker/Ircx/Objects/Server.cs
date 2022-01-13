using System;
using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public class Server: ChatObject
{
    public IAccess Access;
    public IObjectCollection<Channel> Channels;
    public CommandCollection Commands = new();

    // Statistics
    public IObjectCollection<User> Users = new ObjectCollection<User>();

    public Server(IAccess access, IPropStore propStore, IObjectCollection<Channel> channels): base(propStore)
    {
        Channels = channels;
        Properties = propStore;
        this.Access = access;
        this.Name = Name;
        _serverFields = new ServerFields();
        Access = new Access("$", false);
        ServerFields.CreationDate = new string(Resources.GetFullTimeString(Resources.GetTime()));
        var TZ = DateTime.Now.GetDateTimeFormats('R')[0];
        ServerFields.TimeZone = new string(TZ.Substring(TZ.LastIndexOf(' ') + 1));
    }

    //Counting MaxUsers as total concurrent user objects (sockets)
    private readonly ServerFields _serverFields;

    public ServerFields ServerFields
    {
        get { return _serverFields; }
    }

    // Operations
    public User AddUser()
    {
        var user = new User();
        Users.Add(user);
        
        return user;
    }

    public void RemoveUser(User user)
    {
        if (user.Registered)
        {
            user.Unregister();
            if (user.Modes.Invisible.Value == 1)
            {
                user.Modes.Invisible.Value = 0;
            }
        }

        Users.Remove(user);
    }

    public void RegisterUser(User user)
    {
        user.Register();
    }

    public Channel AddChannel(string Name)
    {
        var channel = new Channel(Name);
        Channels.Add(channel);
        return channel;
    }

    public void RemoveChannel(Channel channel)
    {
        Channels.Remove(channel);
    }

    public List<ChatObject> GetObjects(string Nicknames)
    {
        var objs = new List<ChatObject>();
        var NicknameList = Tools.CSVToArray(Nicknames);

        for (var i = 0; i < NicknameList.Count; i++)
        {
            var obj = GetObject(NicknameList[i]);
            if (obj != null) objs.Add(obj);
        }

        return objs;
    }

    public ChatObject GetObject(string Name)
    {
        ChatObject chatObject = null;
        var objIdentifier = IrcHelper.IdentifyObject(Name);


        switch (objIdentifier)
        {
            case IrcHelper.ObjIdentifier.ObjIdExtendedGlobalChannel:
            case IrcHelper.ObjIdentifier.ObjIdExtendedLocalChannel:
            case IrcHelper.ObjIdentifier.ObjIdGlobalChannel:
            case IrcHelper.ObjIdentifier.ObjIdLocalChannel:
            {
                chatObject = Channels.FindObj(Name, objIdentifier);
                break;
            }
            case IrcHelper.ObjIdentifier.ObjIdIRCUser:
            case IrcHelper.ObjIdentifier.ObjIdIRCUserHex:
            case IrcHelper.ObjIdentifier.ObjIdIRCUserUnicode:
            {
                chatObject = Users.FindObj(Name, objIdentifier);
                break;
            }
            case IrcHelper.ObjIdentifier.ObjIdServer:
            {
                chatObject = this;
                break;
            }
        }

        return chatObject;
    }
}