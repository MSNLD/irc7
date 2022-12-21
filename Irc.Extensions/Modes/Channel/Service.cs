using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Service : ModeRule, IModeRule
{
    public Service() : base(ExtendedResources.ChannelModeService)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}