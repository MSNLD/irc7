using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.User
{
    public class Oper : ModeRule, IModeRule
    {
        public Oper() : base(Resources.UserModeOper)
        {
        }

        EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            return EnumModeResult.OK;
        }
    }
}
