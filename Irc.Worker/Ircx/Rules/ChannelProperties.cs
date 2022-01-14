using System;
using System.Collections.Generic;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Helpers.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public class ChannelProperties : PropCollection
{
    public static Prop ClientGuid = new(Resources.ChannelPropClientGuid, string.Empty, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, true);

    public static Prop Creation = new(Resources.ChannelPropCreation, string.Empty, -1, UserAccessLevel.ChatUser,
        UserAccessLevel.NoAccess, true, false);

    public long CreationDate = (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;

    public static Prop Hostkey = new(Resources.ChannelPropHostkey, string.Empty, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public static Prop Lag = new(Resources.ChannelPropLag, string.Empty, 0, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, false);

    public static Prop Language = new(Resources.ChannelPropLanguage, string.Empty, 31, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatHost, false, false);

    public static Prop Memberkey = new(Resources.ChannelPropMemberkey, string.Empty, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public static Prop OnJoin = new(Resources.ChannelPropOnJoin, string.Empty, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public static Prop OnPart = new(Resources.ChannelPropOnPart, string.Empty, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public static Prop Ownerkey = new(Resources.ChannelPropOwnerkey, string.Empty, 31, UserAccessLevel.ChatOwner,
        UserAccessLevel.ChatOwner, false, true);

    public static Prop Subject = new(Resources.ChannelPropSubject, string.Empty, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatSysopManager, true, false);

    public static Prop Topic = new(Resources.ChannelPropTopic, string.Empty, 160, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatHost, false, false);

    public static readonly Dictionary<string, Prop> PropertyRules = new (StringComparer.InvariantCultureIgnoreCase)
        {
            { Resources.ChannelPropClientGuid, ClientGuid },
            { Resources.ChannelPropCreation, Creation },
            { Resources.ChannelPropHostkey, Hostkey },
            { Resources.ChannelPropLag, Lag },
            { Resources.ChannelPropLanguage, Language },
            { Resources.ChannelPropMemberkey, Memberkey },
            { Resources.ChannelPropOnJoin, OnJoin },
            { Resources.ChannelPropOnPart, OnPart },
            { Resources.ChannelPropOwnerkey, Ownerkey },
            { Resources.ChannelPropSubject, Subject },
            { Resources.ChannelPropTopic, Topic }
        };
}