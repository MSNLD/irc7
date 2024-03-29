﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class AuthOnly : ModeRuleChannel, IModeRule
{
    public AuthOnly() : base(ExtendedResources.ChannelModeAuthOnly)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}