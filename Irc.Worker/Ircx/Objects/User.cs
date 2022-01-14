using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Irc.Constants;
using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class User : Client
{
    public Access Access;
    public IDictionary<Channel, ChannelMember> Channels;
    public UserAccessLevel Level;
    public UserModeCollection Modes;
    public Profile Profile;

    public string RealName;

    public User()
    {
        Access = new Access(Resources.Wildcard, false);
        Modes = new UserModeCollection();
        Channels = new ConcurrentDictionary<Channel, ChannelMember>();
        Profile = new Profile();
        UpdateUserNickname(Resources.Wildcard);
    }

    public void BroadcastToChannels(string data, bool ExcludeUser)
    {
        foreach (Channel channel in Channels.Keys)
        {
            channel.Send(data, this, ExcludeUser);
        }
    }
    
    public void AddChannel(Channel channel, ChannelMember member)
    {
        Channels.Add(channel, member);
    }

    public void RemoveChannel(Channel channel)
    {
        Channels.Remove(channel);
    }

    public KeyValuePair<Channel, ChannelMember> GetChannelMemberInfo(Channel channel)
    {
        return Channels.FirstOrDefault(c => c.Key == channel);
    }

    public KeyValuePair<Channel, ChannelMember> GetChannelInfo(string Name)
    {
        return Channels.FirstOrDefault(c => c.Key.Name == Name);
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