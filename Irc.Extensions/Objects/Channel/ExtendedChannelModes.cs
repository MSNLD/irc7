using Irc.Extensions.Modes.Channel;
using Irc.Modes.Channel.Member;
using Irc.Objects;

namespace Irc.Extensions.Objects.Channel;

public class ExtendedChannelModes : ChannelModes
{
    public ExtendedChannelModes()
    {
        modes.Add(ExtendedResources.ChannelModeAuthOnly, new AuthOnly());
        modes.Add(ExtendedResources.ChannelModeProfanity, new NoFormat());
        modes.Add(ExtendedResources.ChannelModeHidden, new Hidden());
        modes.Add(ExtendedResources.ChannelModeRegistered, new Registered());
        modes.Add(ExtendedResources.ChannelModeKnock, new Knock());
        modes.Add(ExtendedResources.ChannelModeNoWhisper, new NoWhisper());
        modes.Add(ExtendedResources.ChannelModeAuditorium, new Auditorium());
        modes.Add(ExtendedResources.ChannelModeCloneable, new Cloneable());
        modes.Add(ExtendedResources.ChannelModeClone, new Clone());
        modes.Add(ExtendedResources.ChannelModeService, new Service());
        modes.Add(ExtendedResources.MemberModeOwner, new Owner());
    }
}