using System;

namespace Core.Ircx.Objects;

public class ChannelProperties : PropCollection
{
    public Prop ClientGuid = new(Resources.ChannelPropClientGuid, Resources.Null, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, true);

    public Prop Creation = new(Resources.ChannelPropCreation, Resources.Null, -1, UserAccessLevel.ChatUser,
        UserAccessLevel.NoAccess, true, false);

    public long CreationDate = (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;

    public Prop Hostkey = new(Resources.ChannelPropHostkey, Resources.Null, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public Prop Lag = new(Resources.ChannelPropLag, Resources.Null, 0, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatOwner, false, false);

    public Prop Language = new(Resources.ChannelPropLanguage, Resources.Null, 31, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatHost, false, false);

    public Prop Memberkey = new(Resources.ChannelPropMemberkey, Resources.Null, 31, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, true);

    public Prop OnJoin = new(Resources.ChannelPropOnJoin, Resources.Null, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public Prop OnPart = new(Resources.ChannelPropOnPart, Resources.Null, 255, UserAccessLevel.ChatHost,
        UserAccessLevel.ChatHost, false, false);

    public Prop Ownerkey = new(Resources.ChannelPropOwnerkey, Resources.Null, 31, UserAccessLevel.ChatOwner,
        UserAccessLevel.ChatOwner, false, true);

    public Prop Subject = new(Resources.ChannelPropSubject, Resources.Null, 32, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatSysopManager, true, false);

    public Prop Topic = new(Resources.ChannelPropTopic, Resources.Null, 160, UserAccessLevel.ChatUser,
        UserAccessLevel.ChatHost, false, false);

    public long TopicLastChanged;

    public ChannelProperties(Obj obj) : base(obj)
    {
        Creation.Value = new string(CreationDate.ToString());
        Language.Value = "1";
        Properties.Add(Creation);
        Properties.Add(Language);
        Properties.Add(Topic);
        Properties.Add(OnJoin);
        Properties.Add(OnPart);
        Properties.Add(Lag); //only display LAG if over 0
        Properties.Add(Subject);
        Properties.Add(Memberkey);
        Properties.Add(Ownerkey);
        Properties.Add(Hostkey);
        Properties.Add(ClientGuid);
    }
}