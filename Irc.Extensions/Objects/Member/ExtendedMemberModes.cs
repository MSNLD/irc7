using Irc.Extensions.Modes.Channel.Member;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Objects.Member;

public class ExtendedMemberModes : global::Irc.Objects.Member.Member, IMemberModes
{
    public ExtendedMemberModes() : base(null)
    {
    }

    public ExtendedMemberModes(IUser user) : base(user)
    {
        modes.Add(ExtendedResources.MemberModeOwner, new Owner());
    }

    public new char GetListedMode()
    {
        if (IsOwner()) return ExtendedResources.MemberModeFlagOwner;

        return base.GetListedMode();
    }

    public new char GetModeChar()
    {
        if (IsOwner()) return ExtendedResources.MemberModeOwner;

        return base.GetModeChar();
    }

    public new bool IsNormal()
    {
        return !IsOwner() && base.IsNormal();
    }

    public new void SetNormal()
    {
        SetOwner(false);
        base.SetNormal();
    }

    public new bool IsOwner()
    {
        return GetModeChar(ExtendedResources.MemberModeOwner) > 0;
    }

    public new void SetOwner(bool flag)
    {
        modes[ExtendedResources.MemberModeOwner].Set(flag ? 1 : 0);
    }
}