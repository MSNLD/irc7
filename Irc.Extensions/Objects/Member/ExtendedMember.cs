﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Member;

namespace Irc.Extensions.Objects.Member;

public class ExtendedMember : ExtendedMemberModes, IChannelMember
{
    public ExtendedMember(IUser User) : base(User)
    {
    }
    public void SetOwner(bool flag)
    {
        modes[Resources.MemberModeOwner].Set(flag ? 1 : 0);
    }
}
