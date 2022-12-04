using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Extensions.Modes.Channel;

public class Knock : ModeRule, IModeRule
{
    public Knock() : base(ExtendedResources.ChannelModeKnock)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}