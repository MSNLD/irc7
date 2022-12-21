using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Hidden : ModeRule, IModeRule
{
    public Hidden() : base(ExtendedResources.ChannelModeHidden)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}