using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Objects.Member;

public class Member : MemberModes, IChannelMember
{
    private readonly User _user;

    public Member(User User)
    {
        _user = User;
    }

    public EnumUserAccessLevel GetLevel()
    {
        if (_user.Level >= EnumUserAccessLevel.ChatGuide) return _user.Level;

        //if (IsOwner())
        //    return EnumUserAccessLevel.ChatOwner;
        if (IsHost())
            return EnumUserAccessLevel.ChatHost;

        if (IsVoice())
            return EnumUserAccessLevel.ChatVoice;

        return _user.Guest ? EnumUserAccessLevel.ChatGuest : EnumUserAccessLevel.ChatMember;
    }

    public User GetUser()
    {
        return _user;
    }
}