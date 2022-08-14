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
    public class NoExtern : ModeRule, IModeRule
    {
        public NoExtern() : base(Resources.ChannelModeNoExtern)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            return EnumIrcError.OK;
        }
    }
}
