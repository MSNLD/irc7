using System.Text.RegularExpressions;
using Irc.Commands;
using Irc.Constants;
using Irc.Interfaces;
using Irc.IO;

namespace Irc.Objects.Channel;

public class Channel : ChatObject, IChannel
{
    private readonly IModeCollection _modes;
    private readonly IList<IChannelMember> _members = new List<IChannelMember>();
    public IList<Address> BanList = new List<Address>();
    public IList<User> InviteList = new List<User>();

    // TODO: The ‘l’, ‘b’, ‘k’ mode is stored with the Channel Store.
    public IDataStore ChannelStore => DataStore;

    public Channel(string name, IModeCollection modes, IDataStore dataStore) : base(dataStore)
    {
        _modes = modes;
        SetName(name);
        DataStore.SetId(Name);
    }

    public string GetName()
    {
        return Name;
    }

    public IChannelMember GetMember(User User)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetUser() == User)
                return channelMember;

        return null;
    }

    public bool Allows(User user)
    {
        if (IsOnChannel(user)) return false;
        return true;
    }

    public IChannel Join(User user)
    {
        var member = new Member.Member(user);
        _members.Add(member);
        Send(Raw.RPL_JOIN_IRC(user, this));
        return this;
    }

    public IChannel SendTopic(User user)
    {
        user.Send(Raw.IRCX_RPL_TOPIC_332(user.Server, user, this, DataStore.Get("topic")));
        return this;
    }

    public IChannel SendNames(User user)
    {
        Names.ProcessNamesReply(user, this);
        return this;
    }

    public IChannel Part(User user)
    {
        var member = _members.Where(m => m.GetUser() == user).FirstOrDefault();
        Send(Raw.RPL_PART_IRC(user, this));
        _members.Remove(member);
        return this;
    }

    public void SendMessage(User user, string message)
    {
        Send(Raw.RPL_PRIVMSG_CHAN(user, this, message), user, true);
    }

    public void Send(string message, User u, bool ExcludeSender)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetUser() != u || !ExcludeSender)
                channelMember.GetUser().Send(message);
    }

    public void Send(string message, User u)
    {
        throw new NotImplementedException();
    }

    public IList<IChannelMember> GetMembers() => _members;

    public IModeCollection GetModes() => _modes;

    public void SetName(string Name)
    {
        this.Name = Name;
    }

    public bool IsOnChannel(User user)
    {
        foreach (var member in _members)
            // TODO: Re-enable below
            //if (CompareUserAddress(user, member.GetUser())) return true;
            if (CompareUserNickname(member.GetUser(), user))
                return true;

        return false;
    }

    private static bool CompareUserAddress(User user, User otherUser)
    {
        if (otherUser == user || otherUser.Address.GetUserHost() == user.Address.GetUserHost()) return true;
        return false;
    }

    private static bool CompareUserNickname(User user, User otherUser)
    {
        return otherUser.Address.Nickname.ToUpper() == user.Address.Nickname.ToUpper();
    }

    public static bool ValidName(string channel)
    {
        var regex = new Regex(Resources.ChannelRegEx);
        return regex.Match(channel).Success;
    }

    public void Send(string message)
    {
        Send(message, null, false);
    }
}