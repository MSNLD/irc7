using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel
{
    internal class Key : ModeRuleChannel, IModeRule
    {
        public Key() : base(Resources.ChannelModeKey, true)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            IChannelMember member = (IChannelMember)source;
            if (member.IsHost())
            {
                return EnumIrcError.OK;
            }
            else
            {
                /* -> sky-8a15b323126 MODE #test +t
                <- :sky-8a15b323126 482 Sky2k #test :You're not channel operator */
                return EnumIrcError.ERR_NOCHANOP;
            }
        }
    }
}
