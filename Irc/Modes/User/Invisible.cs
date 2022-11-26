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

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            if (source == target)
            {
                target.Modes[Resources.UserModeInvisible].Set(flag);
                DispatchModeChange(source, target, flag, parameter);
                return EnumIrcError.OK;
            }
            else return EnumIrcError.ERR_NOSUCHCHANNEL;
        }
    }
}
