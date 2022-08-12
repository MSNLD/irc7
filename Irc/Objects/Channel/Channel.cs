using System.Text.RegularExpressions;
using Irc.Commands;
using Irc.Constants;
using Irc.Interfaces;
using Irc.IO;

namespace Irc.Objects.Channel;

public class Channel : ChatObject, IChannel
{
    private readonly IList<IChannelMember> _members = new List<IChannelMember>();
    public IList<Address> BanList = new List<Address>();
    public IList<User> InviteList = new List<User>();

    public Channel(string name, IModeCollection modes, IDataStore dataStore) : base(modes, dataStore)
    {
        SetName(name);
        DataStore.SetId(Name);
    }

    // TODO: The ‘l’, ‘b’, ‘k’ mode is stored with the Channel Store.
    public IDataStore ChannelStore => DataStore;

    public string GetName()
    {
        return Name;
    }

    public IChannelMember GetMember(IUser User)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetUser() == User)
                return channelMember;

        return null;
    }

    public IChannelMember GetMemberByNickname(string nickname) => _members.FirstOrDefault(member => string.Compare(member.GetUser().GetAddress().Nickname, nickname, true) == 0);

    public bool Allows(IUser user)
    {
        if (IsOnChannel(user)) return false;
        return true;
    }

    public IChannel Join(IUser user)
    {
        AddMember(user);
        Send(IrcRaws.RPL_JOIN(user, this));
        return this;
    }

    private IChannelMember AddMember(IUser user)
    {
        var member = new Member.Member(user);
        _members.Add(member);
        user.AddChannel(this, member);
        return member;
    }

    private void RemoveMember(IUser user)
    {
        var member = _members.Where(m => m.GetUser() == user).FirstOrDefault();
        _members.Remove(member);
    }


    public IChannel SendTopic(IUser user)
    {
        user.Send(Raw.IRCX_RPL_TOPIC_332(user.Server, user, this, DataStore.Get("topic")));
        return this;
    }

    public IChannel SendNames(IUser user)
    {
        Names.ProcessNamesReply(user, this);
        return this;
    }

    public IChannel Part(IUser user)
    {
        RemoveMember(user);
        Send(IrcRaws.RPL_PART(user, this));
        return this;
    }

    public IChannel Quit(IUser user)
    {
        RemoveMember(user);
        return this;
    }

    public void SendMessage(IUser user, string message)
    {
        Send(IrcRaws.RPL_PRIVMSG(user, this, message), user, true);
    }

    public void SendNotice(IUser user, string message)
    {
        Send(IrcRaws.RPL_NOTICE(user, this, message), user, true);
    }

    public void Send(string message, IUser u, bool ExcludeSender)
    {
        foreach (var channelMember in _members)
            if (channelMember.GetUser() != u || !ExcludeSender)
                channelMember.GetUser().Send(message);
    }

    public void Send(string message, IUser u)
    {
        throw new NotImplementedException();
    }

    public IList<IChannelMember> GetMembers()
    {
        return _members;
    }

    public IModeCollection GetModes()
    {
        return _modes;
    }

    public void SetName(string Name)
    {
        this.Name = Name;
    }

    public bool IsOnChannel(IUser user)
    {
        foreach (var member in _members)
            // TODO: Re-enable below
            //if (CompareUserAddress(user, member.GetUser())) return true;
            if (CompareUserNickname(member.GetUser(), user))
                return true;

        return false;
    }

    private static bool CompareUserAddress(IUser user, IUser otherUser)
    {
        if (otherUser == user || otherUser.GetAddress().GetUserHost() == user.GetAddress().GetUserHost()) return true;
        return false;
    }

    private static bool CompareUserNickname(IUser user, IUser otherUser)
    {
        return otherUser.GetAddress().Nickname.ToUpper() == user.GetAddress().Nickname.ToUpper();
    }

    public static bool ValidName(string channel)
    {
        var regex = new Regex(Resources.IrcChannelRegex);
        return regex.Match(channel).Success;
    }

    public void Send(string message)
    {
        Send(message, null, false);
    }
}