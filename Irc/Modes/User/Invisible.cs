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
    public class Invisible : ModeRule, IModeRule
    {
        public Invisible() : base(Resources.UserModeInvisible)
        {
        }

        EnumModeResult Evaluate(ChatObject chatObject, string modeValue)
        {
            return EnumModeResult.OK;
        }
    }
}
