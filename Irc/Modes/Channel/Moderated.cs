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
    public class Moderated : ModeRule, IModeRule
    {
        public Moderated() : base(Resources.ChannelModeModerated)
        {
        }

        EnumModeResult Evaluate(ChatObject chatObject, string modeValue)
        {
            return EnumModeResult.OK;
        }
    }
}
