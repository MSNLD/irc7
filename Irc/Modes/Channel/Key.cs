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
    internal class Key : ModeRule, IModeRule
    {
        public Key() : base(Resources.ChannelModeKey, true)
        {
        }

        public new EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            IChannelMember member = (IChannelMember)source;
            IUser user = member.GetUser();
            if (source.Level >= EnumUserAccessLevel.ChatHost || user.GetLevel() > EnumUserAccessLevel.ChatHost)
            {
                return EnumModeResult.OK;
            }
            else
            {
                /* -> sky-8a15b323126 MODE #test +t
                <- :sky-8a15b323126 482 Sky2k #test :You're not channel operator */
                return EnumModeResult.NOTOPER;
            }
        }
    }
}
