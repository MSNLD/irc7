using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Registered : ModeRule, IModeRule
{
    public Registered() : base(ExtendedResources.ChannelModeRegistered)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}