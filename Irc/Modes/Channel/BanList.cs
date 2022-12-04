using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes.Channel;

public class BanList : ModeRule, IModeRule
{
    public BanList() : base(Resources.ChannelModeBan)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}