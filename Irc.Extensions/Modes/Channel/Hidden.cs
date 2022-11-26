using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Modes.Channel
{
    public class Hidden : ModeRule, IModeRule
    {
        public Hidden() : base(ExtendedResources.ChannelModeHidden)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            return EnumIrcError.OK;
        }
    }
}
