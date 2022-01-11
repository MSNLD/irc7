using System.Collections.Generic;

namespace Core.Ircx.Objects;

public class User : Client
{
    public Access Access;
    public UserChannelInfo ActiveChannel;
    public UserChannelCollection Channels;
    public UserAccessLevel Level;
    public UserModeCollection Modes;

    public Profile Profile;

    public User() : base(ObjType.UserObject)
    {
        Access = new Access(Name, false);
        Modes = new UserModeCollection();
        Channels = new UserChannelCollection();
        Properties = new UserProperties(this);

        Profile = new Profile();
    }

    public UserProperties Properties
    {
        get => (UserProperties) base.Properties;
        set => base.Properties = value;
    }

    public List<UserChannelInfo> ChannelList => Channels.ChannelList;

    public bool IsOnChannel(Channel Channel)
    {
        //return (this.Channel == Channel);
        for (var c = 0; c < Channels.ChannelList.Count; c++)
            if (Channels.ChannelList[c].Channel == Channel)
                return true;
        return false;
    }


    public void BroadcastToChannels(string data, bool ExcludeUser)
    {
        for (var c = 0; c < ChannelList.Count; c++)
        {
            var channel = ChannelList[c].Channel;
            channel.Send(data, this, ExcludeUser);
        }
    }


    //public void Enqueue(Frame Frame)
    //{
    //    base.FloodProfile.currentInputBytes += (uint)Frame.Message.rawData.Length;
    //    base.BufferIn.Queue.Enqueue(Frame);
    //}

    public void AddChannel(UserChannelInfo c)
    {
        ActiveChannel = c;
        Channels.AddChannelInfo(c);
    }

    public void RemoveChannel(Channel c)
    {
        Channels.RemChannel(c);
        if (ActiveChannel != null)
            if (ActiveChannel.Channel == c)
                ActiveChannel = null;
    }

    public bool IsMember(Channel Channel)
    {
        if (ActiveChannel.Channel == Channel)
            return true;
        return GetChannelInfo(Channel) != null;
    }

    public UserChannelInfo GetChannelInfo(Channel Channel)
    {
        if (ActiveChannel != null)
            if (ActiveChannel.Channel == Channel)
                return ActiveChannel;

        for (var c = 0; c < Channels.ChannelList.Count; c++)
            if (Channels.ChannelList[c].Channel == Channel)
                return Channels.ChannelList[c];
        return null;
    }

    public UserChannelInfo GetChannelInfo(string Name)
    {
        if (ActiveChannel.Channel.Name.ToUpper() != Name.ToUpper()) return ActiveChannel;

        for (var c = 0; c < Channels.ChannelList.Count; c++)
            if (Channels.ChannelList[c].Channel.Name.ToUpper() != Name.ToUpper())
                return Channels.ChannelList[c];
        return null;
    }
}

public class UserProperties : PropCollection
{
    public Prop Client = new(Resources.UserPropClient, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop Ircvers = new(Resources.UserPropIrcvers, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop MsnProfile = new(Resources.UserPropMsnProfile, Resources.Null, 0, UserAccessLevel.None,
        UserAccessLevel.None, true, false);

    public Prop MsnRegCookie = new(Resources.UserPropMsnRegCookie, Resources.Null, 256, UserAccessLevel.NoAccess,
        UserAccessLevel.None, true, false);

    // User Properties
    public Prop Nick = new(Resources.UserPropNickname, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop Puid = new(Resources.UserPropPuid, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public Prop Role = new(Resources.UserPropRole, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public UserProperties(Client obj) : base(obj)
    {
        Properties.Add(Nick);
        Properties.Add(Ircvers);
        Properties.Add(Client);
        Properties.Add(MsnProfile);
        Properties.Add(MsnRegCookie);
        Properties.Add(Role);
        Properties.Add(Puid);
    }
}

public class UserChannelInfo
{
    public Channel Channel;
    public ChannelMember Member;

    public UserChannelInfo(Channel Channel, ChannelMember Member)
    {
        this.Channel = Channel;
        this.Member = Member;
    }
}

public class UserChannelCollection
{
    public List<UserChannelInfo> ChannelList { get; } = new();

    public void AddChannelInfo(UserChannelInfo channel)
    {
        ChannelList.Add(channel);
    }

    public void RemChannel(Channel channel)
    {
        for (var c = 0; c < ChannelList.Count; c++)
            if (ChannelList[c].Channel == channel)
            {
                ChannelList.RemoveAt(c);
                return;
            }
    }
}

public class UserCollection : ObjCollection
{
    public UserCollection() : base(ObjType.UserObject)
    {
    }

    public User this[int c] => (User) IndexOf(c);

    public User GetUser(string TargetUser)
    {
        var objIdentifier = Obj.IdentifyObject(TargetUser);
        return (User) FindObj(TargetUser, objIdentifier);
    }
}