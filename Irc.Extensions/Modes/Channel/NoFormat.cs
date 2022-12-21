using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class NoFormat : ModeRule, IModeRule
{
    public NoFormat() : base(ExtendedResources.ChannelModeProfanity)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}