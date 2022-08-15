using Irc.Objects;

namespace Irc.Extensions.Objects.Channel;

public class ExtendedChannelModes : ChannelModes
{
    public ExtendedChannelModes(): base()
    {
        modes.Add(ExtendedResources.ChannelModeAuthOnly, new Modes.Channel.AuthOnly());
        modes.Add(ExtendedResources.ChannelModeProfanity, new Modes.Channel.NoFormat());
        modes.Add(ExtendedResources.ChannelModeHidden, new Modes.Channel.Hidden());
        modes.Add(ExtendedResources.ChannelModeRegistered, new Modes.Channel.Registered());
        modes.Add(ExtendedResources.ChannelModeKnock, new Modes.Channel.Knock());
        modes.Add(ExtendedResources.ChannelModeNoWhisper, new Modes.Channel.NoWhisper());
        modes.Add(ExtendedResources.ChannelModeAuditorium, new Modes.Channel.Auditorium());
        modes.Add(ExtendedResources.ChannelModeCloneable, new Modes.Channel.Cloneable());
        modes.Add(ExtendedResources.ChannelModeClone, new Modes.Channel.Clone());
        modes.Add(ExtendedResources.ChannelModeService, new Modes.Channel.Service());
        modes.Add(ExtendedResources.MemberModeOwner, new global::Irc.Modes.Channel.Member.Owner());
    }
}