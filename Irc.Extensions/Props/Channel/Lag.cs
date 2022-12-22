﻿using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Lag : PropRule
{
    // The LAG channel property contains a numeric value between 0 to 2 seconds.
    // The server will add an artificial delay of that length between subsequent messages from the same member.
    // All messages to the channel are affected. 
    public Lag() : base(ExtendedResources.ChannelPropLag, EnumChannelAccessLevel.ChatHost,
        EnumChannelAccessLevel.ChatHost, string.Empty)
    {
    }
}