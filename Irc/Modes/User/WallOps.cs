﻿using Irc.Constants;
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
    public class WallOps : ModeRule, IModeRule
    {
        public WallOps() : base(Resources.UserModeWallops)
        {
        }

        EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            return EnumModeResult.OK;
        }
    }
}