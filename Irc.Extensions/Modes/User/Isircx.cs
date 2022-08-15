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

namespace Irc.Extensions.Modes.User
{
    public class Isircx : ModeRule, IModeRule
    {
        public Isircx() : base(ExtendedResources.UserModeIrcx)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            return EnumIrcError.ERR_UNKNOWNMODEFLAG;
        }
    }
}
