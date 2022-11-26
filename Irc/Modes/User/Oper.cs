using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
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

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            // :sky-8a15b323126 908 Sky :No permissions to perform command
            if (source is IUser && ((IUser)source).IsSysop() && flag == false)
            {
                target.Modes[Resources.UserModeOper].Set(flag);
                DispatchModeChange(source, target, flag, parameter);
                return EnumIrcError.OK;
            }
            else return EnumIrcError.ERR_NOPERMS;
        }
    }
}
