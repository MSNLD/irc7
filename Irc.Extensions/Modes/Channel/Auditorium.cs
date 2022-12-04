using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Extensions.Modes.Channel;

public class Auditorium : ModeRule, IModeRule
{
    public Auditorium() : base(ExtendedResources.ChannelModeAuditorium)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}