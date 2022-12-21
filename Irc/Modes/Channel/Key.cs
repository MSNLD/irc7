using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Objects;

namespace Irc.Modes.Channel;

internal class Key : ModeRule, IModeRule
{
    public Key() : base(Resources.ChannelModeKey, true)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        var member = (IChannelMember)source;
        if (member.IsHost())
            return EnumIrcError.OK;
        /* -> sky-8a15b323126 MODE #test +t
            <- :sky-8a15b323126 482 Sky2k #test :You're not channel operator */
        return EnumIrcError.ERR_NOCHANOP;
    }
}