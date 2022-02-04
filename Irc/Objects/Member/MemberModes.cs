using Irc.Constants;
using Irc.Interfaces;

namespace Irc.Objects;

public class MemberModes : ModeCollection, IMemberModes
{
    public MemberModes()
    {
        modes.Add(Resources.MemberModeHost, 0);
        modes.Add(Resources.MemberModeVoice, 0);
    }

    public string GetModeString()
    {
        return modes?.ToString()!;
    }

    public char GetListedMode()
    {
        if (IsHost()) return Resources.MemberModeFlagHost;
        if (IsVoice()) return Resources.MemberModeFlagVoice;

        return (char) 0;
    }

    public char GetModeChar()
    {
        if (IsHost()) return Resources.MemberModeHost;
        if (IsVoice()) return Resources.MemberModeVoice;

        return (char) 0;
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
        modes[Resources.MemberModeHost] = flag ? 1 : 0;
    }

    public void SetVoice(bool flag)
    {
        modes[Resources.MemberModeVoice] = flag ? 1 : 0;
    }

    public void SetNormal()
    {
        SetHost(false);
        SetVoice(false);
    }
}