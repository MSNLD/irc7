using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Cloneable : ModeRule, IModeRule
{
    public Cloneable() : base(ExtendedResources.ChannelModeCloneable)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}