﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes;

namespace Irc.Extensions.Modes.Channel;

public class Auditorium : ModeRuleChannel, IModeRule
{
    public Auditorium() : base(ExtendedResources.ChannelModeAuditorium)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        return EvaluateAndSet(source, target, flag, parameter);
    }
}