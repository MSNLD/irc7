﻿using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Objects;

public class ExtendedMemberModes : global::Irc.Objects.Member.Member, IMemberModes
{
    public ExtendedMemberModes(): base(null)
    {

    }
    public ExtendedMemberModes(IUser User): base(User)
    {
        modes.Add(ExtendedResources.MemberModeOwner, new global::Irc.Modes.Channel.Member.Owner());
    }

    public new string GetListedMode()
    {
        if (IsOwner()) return ExtendedResources.MemberModeFlagOwner.ToString();

        return base.GetListedMode();
    }

    public new char GetModeChar()
    {
        if (IsOwner()) return ExtendedResources.MemberModeOwner;

        return base.GetModeChar();
    }

    public bool IsNormal()
    {
        return !IsOwner() && base.IsNormal();
    }

    public new void SetNormal()
    {
        SetOwner(false);
        base.SetNormal();
    }

    public bool IsOwner()
    {
        return GetModeChar(ExtendedResources.MemberModeOwner) > 0;
    }

    public void SetOwner(bool flag)
    {
        modes[ExtendedResources.MemberModeOwner].Set(flag ? 1 : 0);
    }
}