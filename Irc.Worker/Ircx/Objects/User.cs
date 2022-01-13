using System.Collections.Generic;
using Irc.Constants;
using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class User : Client
{
    public Access Access;
    public UserChannelInfo ActiveChannel;
    public UserChannelCollection Channels;
    public UserAccessLevel Level;
    public UserModeCollection Modes;
    public Profile Profile;

    public User()
    {
        Access = new Access(Name, false);
        Modes = new UserModeCollection();
        Channels = new UserChannelCollection();
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
        if (ActiveChannel.Channel.Name.ToUpper() == Name.ToUpper()) return ActiveChannel;

        for (var c = 0; c < Channels.ChannelList.Count; c++)
            if (Channels.ChannelList[c].Channel.Name.ToUpper() == Name.ToUpper())
                return Channels.ChannelList[c];
        return null;
    }

    public void UpdateUserNickname(string Nickname)
    {
        // if OK
        Address.Nickname = Nickname;
        Access.ObjectName = Nickname;
        Properties.Set(Resources.UserPropNickname, Nickname);
        Name = Nickname;
    }
}