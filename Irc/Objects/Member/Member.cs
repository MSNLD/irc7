using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Objects.Member;

public class Member : MemberModes, IChannelMember
{
    protected readonly IUser _user;

    public Member(IUser User)
    {
        _user = User;
    }

    public EnumChannelAccessLevel GetLevel()
    {
        if (IsHost())
            return EnumChannelAccessLevel.ChatHost;

        if (IsVoice())
            return EnumChannelAccessLevel.ChatVoice;

        return _user.IsGuest() ? EnumChannelAccessLevel.ChatGuest : EnumChannelAccessLevel.ChatMember;
    }

    public EnumIrcError CanModify(IChannelMember target, EnumChannelAccessLevel requiredLevel)
    {
        // Oper check
        if (target.GetUser().GetLevel() >= EnumUserAccessLevel.Guide)
        {
            if (_user.GetLevel() < EnumUserAccessLevel.Guide) return EnumIrcError.ERR_NOIRCOP;
            // TODO: Maybe there is better raws for below
            else if (_user.GetLevel() < EnumUserAccessLevel.Sysop && _user.GetLevel() < target.GetUser().GetLevel()) return EnumIrcError.ERR_NOPERMS;
            else if (_user.GetLevel() < EnumUserAccessLevel.Administrator && _user.GetLevel() < target.GetUser().GetLevel()) return EnumIrcError.ERR_NOPERMS;
        }

        if (!IsOwner() && requiredLevel >= EnumChannelAccessLevel.ChatOwner) return EnumIrcError.ERR_NOCHANOWNER;
        else if (!IsHost() && requiredLevel >= EnumChannelAccessLevel.ChatHost) return EnumIrcError.ERR_NOCHANOP;
        else return EnumIrcError.OK;
    }

    public IUser GetUser()
    {
        return _user;
    }
}