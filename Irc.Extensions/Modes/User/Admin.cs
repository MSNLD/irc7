﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Extensions.Modes.User;

public class Admin : ModeRule, IModeRule
{
    public Admin() : base(ExtendedResources.UserModeAdmin)
    {
    }

    public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
    {
        // :sky-8a15b323126 908 Sky :No permissions to perform command
        if (source is IUser && ((IUser)source).IsAdministrator() && flag == false)
        {
            target.Modes[ExtendedResources.UserModeAdmin].Set(flag);
            DispatchModeChange(source, target, flag, parameter);
            return EnumIrcError.OK;
        }

        return EnumIrcError.ERR_NOPERMS;
    }
}