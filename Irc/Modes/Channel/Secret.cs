using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Objects;

namespace Irc.Modes.Channel;

public class Secret : ModeRule, IModeRule
{
    public Secret() : base(Resources.ChannelModeSecret)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}