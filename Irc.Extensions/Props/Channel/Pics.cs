﻿using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Pics : PropRule
{
    // The PICS channel property is the current PICS rating of the channel.
    // Chat clients that are PICS enabled can use this property to determine if the channel is appropriate for the user.
    // The PICS property is limited to 255 characters.
    // This property may be set by sysop managers and read by all. It may not be read by ordinary users if the channel is SECRET.
    public Pics() : base(ExtendedResources.ChannelPropPics, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, string.Empty, true)
    {
    }
}