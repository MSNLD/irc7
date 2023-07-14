using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Modes.Channel;

public class BanList : ModeRuleChannel, IModeRule
{
    public BanList() : base(Resources.ChannelModeBan)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        //return EvaluateAndSet(source, target, flag, parameter);
        return EnumIrcError.OK;
    }
}