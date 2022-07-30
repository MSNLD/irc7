using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel.Member
{
    public class Operator : ModeRule, IModeRule
    {
        public Operator() : base(Resources.UserModeOper)
        {
        }

        EnumModeResult Evaluate(ChatObject chatObject, string modeValue)
        {
            return EnumModeResult.OK;
        }
    }
}
