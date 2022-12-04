using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Extensions.Modes.Channel;

public class Service : ModeRule, IModeRule
{
    public Service() : base(ExtendedResources.ChannelModeService)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        return EnumIrcError.OK;
    }
}