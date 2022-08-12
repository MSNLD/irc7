﻿using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Objects.Member;

public class Member : MemberModes, IChannelMember
{
    private readonly IUser _user;

    public Member(IUser User)
    {
        _user = User;
    }

    public EnumUserAccessLevel GetLevel()
    {
        if (IsHost())
            return EnumUserAccessLevel.ChatHost;

        if (IsVoice())
            return EnumUserAccessLevel.ChatVoice;

        return _user.IsGuest() ? EnumUserAccessLevel.ChatGuest : EnumUserAccessLevel.ChatMember;
    }

    public IUser GetUser()
    {
        return _user;
    }
}