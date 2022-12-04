using Irc.Constants;
using Irc.Interfaces;
using Irc.Modes.Channel.Member;

namespace Irc.Objects;

public class MemberModes : ModeCollection, IMemberModes
{
    public MemberModes()
    {
        modes.Add(Resources.MemberModeHost, new Operator());
        modes.Add(Resources.MemberModeVoice, new Voice());
    }

    public string GetModeString()
    {
        return modes?.ToString()!;
    }

    public char GetListedMode()
    {
        if (IsOwner()) return Resources.MemberModeFlagOwner;
        if (IsHost()) return Resources.MemberModeFlagHost;
        if (IsVoice()) return Resources.MemberModeFlagVoice;

        return (char)0;
    }

    public char GetModeChar()
    {
        if (IsOwner()) return Resources.MemberModeOwner;
        if (IsHost()) return Resources.MemberModeHost;
        if (IsVoice()) return Resources.MemberModeVoice;

        return (char)0;
    }

    public bool IsOwner()
    {
        // TODO: Need to think about a better way of handling the below
        return modes.ContainsKey(Resources.MemberModeOwner) && GetModeChar(Resources.MemberModeOwner) > 0;
    }

    public bool IsHost()
    {
        return GetModeChar(Resources.MemberModeHost) > 0;
    }

    public bool IsVoice()
    {
        return GetModeChar(Resources.MemberModeVoice) > 0;
    }

    public bool IsNormal()
    {
        return !IsHost() && !IsVoice();
    }

    public void SetHost(bool flag)
    {
        modes[Resources.MemberModeHost].Set(flag ? 1 : 0);
    }

    public void SetVoice(bool flag)
    {
        modes[Resources.MemberModeVoice].Set(flag ? 1 : 0);
    }

    public void SetNormal()
    {
        SetHost(false);
        SetVoice(false);
    }

    public void SetOwner(bool flag)
    {
        throw new NotImplementedException();
    }
}