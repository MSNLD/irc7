using System;
using System.Collections.Generic;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Helpers.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public class ChannelProperties : PropCollection
{
    public static Prop ClientGuid = new(Resources.ChannelPropClientGuid, Resources.Null, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, true);

    public static Prop Creation = new(Resources.ChannelPropCreation, Resources.Null, -1, UserAccessLevel.ChatUser,
        UserAccessLevel.NoAccess, true, false);

    public long CreationDate = (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;

    public static Prop Hostkey = new(Resources.ChannelPropHostkey, Resources.Null, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public static Prop Lag = new(Resources.ChannelPropLag, Resources.Null, 0, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, false);

    public static Prop Language = new(Resources.ChannelPropLanguage, Resources.Null, 31, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatHost, false, false);

    public static Prop Memberkey = new(Resources.ChannelPropMemberkey, Resources.Null, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public static Prop OnJoin = new(Resources.ChannelPropOnJoin, Resources.Null, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public static Prop OnPart = new(Resources.ChannelPropOnPart, Resources.Null, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public static Prop Ownerkey = new(Resources.ChannelPropOwnerkey, Resources.Null, 31, UserAccessLevel.ChatOwner,
        UserAccessLevel.ChatOwner, false, true);

    public static Prop Subject = new(Resources.ChannelPropSubject, Resources.Null, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatSysopManager, true, false);

    public static Prop Topic = new(Resources.ChannelPropTopic, Resources.Null, 160, UserAccessLevel.ChatUser,
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

    public long TopicLastChanged;

    public ChannelProperties()
    {
        foreach (string prop in PropertyRules.Keys)
        {
            Set(prop, null);
        }
        Set("Creation", CreationDate.ToString());
        Set("Language", "1");
    }
}