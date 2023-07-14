using Irc.Extensions.Interfaces;
using Irc.Extensions.Modes.Channel;
using Irc.Modes.Channel.Member;
using Irc.Objects;

namespace Irc.Extensions.Objects.Channel;

public class ExtendedChannelModes : ChannelModes, IExtendedChannelModes
{
    public ExtendedChannelModes()
    {
        modes.Add(ExtendedResources.ChannelModeAuthOnly, new AuthOnly());
        modes.Add(ExtendedResources.ChannelModeProfanity, new NoFormat());
        modes.Add(ExtendedResources.ChannelModeRegistered, new Registered());
        modes.Add(ExtendedResources.ChannelModeKnock, new Knock());
        modes.Add(ExtendedResources.ChannelModeNoWhisper, new NoWhisper());
        modes.Add(ExtendedResources.ChannelModeAuditorium, new Auditorium());
        modes.Add(ExtendedResources.ChannelModeCloneable, new Cloneable());
        modes.Add(ExtendedResources.ChannelModeClone, new Clone());
        modes.Add(ExtendedResources.ChannelModeService, new Service());
        modes.Add(ExtendedResources.MemberModeOwner, new Owner());
    }

    public bool Auditorium
    {
        get => modes[ExtendedResources.ChannelModeAuditorium].Get() == 1;
        set => modes[ExtendedResources.ChannelModeAuditorium].Set(Convert.ToInt32(value));
    }

    public bool AuthOnly
    {
        get => modes[ExtendedResources.ChannelModeAuthOnly].Get() == 1;
        set => modes[ExtendedResources.ChannelModeAuthOnly].Set(Convert.ToInt32(value));
    }

    public bool Profanity
    {
        get => modes[ExtendedResources.ChannelModeProfanity].Get() == 1;
        set => modes[ExtendedResources.ChannelModeProfanity].Set(Convert.ToInt32(value));
    }

    public bool Registered
    {
        get => modes[ExtendedResources.ChannelModeRegistered].Get() == 1;
        set => modes[ExtendedResources.ChannelModeRegistered].Set(Convert.ToInt32(value));
    }

    public bool Knock
    {
        get => modes[ExtendedResources.ChannelModeKnock].Get() == 1;
        set => modes[ExtendedResources.ChannelModeKnock].Set(Convert.ToInt32(value));
    }

    public bool NoWhisper
    {
        get => modes[ExtendedResources.ChannelModeNoWhisper].Get() == 1;
        set => modes[ExtendedResources.ChannelModeNoWhisper].Set(Convert.ToInt32(value));
    }

    public bool Cloneable
    {
        get => modes[ExtendedResources.ChannelModeCloneable].Get() == 1;
        set => modes[ExtendedResources.ChannelModeCloneable].Set(Convert.ToInt32(value));
    }

    public bool Clone
    {
        get => modes[ExtendedResources.ChannelModeClone].Get() == 1;
        set => modes[ExtendedResources.ChannelModeClone].Set(Convert.ToInt32(value));
    }

    public bool Service
    {
        get => modes[ExtendedResources.ChannelModeService].Get() == 1;
        set => modes[ExtendedResources.ChannelModeService].Set(Convert.ToInt32(value));
    }
}