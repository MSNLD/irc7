﻿using Irc.Constants;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Objects.Member;

public class ExtendedMember : ExtendedMemberModes, IChannelMember
{
    public ExtendedMember(IUser user) : base(user)
    {
    }

    public new void SetOwner(bool flag)
    {
        modes[Resources.MemberModeOwner].Set(flag ? 1 : 0);
    }
}