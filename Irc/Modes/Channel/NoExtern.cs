using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes.Channel;

public class NoExtern : ModeRule, IModeRule
{
    public NoExtern() : base(Resources.ChannelModeNoExtern)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}