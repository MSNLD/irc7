﻿using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Modes.User;

public class Invisible : ModeRule, IModeRule
{
    public Invisible() : base(Resources.UserModeInvisible)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        if (source == target)
        {
            target.Modes[Resources.UserModeInvisible].Set(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOSUCHCHANNEL;
    }
}