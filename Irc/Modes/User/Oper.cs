﻿using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Objects;

namespace Irc.Modes.User;

public class Oper : ModeRule, IModeRule
{
    public Oper() : base(Resources.UserModeOper)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        // :sky-8a15b323126 908 Sky :No permissions to perform command
        if (source is IUser && ((IUser)source).IsSysop() && flag == false)
        {
            target.Modes[Resources.UserModeOper].Set(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOPERMS;
    }
}