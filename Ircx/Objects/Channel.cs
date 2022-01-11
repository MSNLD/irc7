using System;
using System.Collections.Generic;
using System.Globalization;
using Core.Authentication.Package;
using Core.CSharpTools;
using CSharpTools;

namespace Core.Ircx.Objects;

public class Channel : Obj
{
    public Access Access;
    public FloodProfile FloodProfile = new();
    public UserCollection InviteList = new();
    public ChannelMemberCollection Members = new();
    public ChannelModeCollection Modes;

    public Channel(string Name) : base(ObjType.ChannelObject)
    {
        Properties = new ChannelProperties(this);
        Access = new Access(Name, true);
        Modes = new ChannelModeCollection();
        SetName(Name);
    }

    public ChannelProperties Properties
    {
        get => (ChannelProperties) base.Properties;
        set => base.Properties = value;
    }

    public List<ChannelMember> MemberList => Members.MemberList;

    public bool CanProcess => Flood.Audit(FloodProfile, CommandDataType.None, UserAccessLevel.None) == FLD_RESULT.S_OK
        ? true
        : false;

    public bool Contains(User User)
    {
        return Members.IsMember(User);
    }

    private void SetName(string Name)
    {
        this.Name = Name;
    }

    public void AddMember(ChannelMember User)
    {
        Members.AddMember(User);
    }

    public void RemoveMember(User User)
    {
        Members.RemoveMember(User);
    }

    public void RemoveMember(ChannelMember Member)
    {
        Members.RemoveMember(Member.User);
    }

    public ChannelMember GetMember(User User)
    {
        for (var c = 0; c < Members.MemberList.Count; c++)
            if (Members.MemberList[c].User == User)
                return Members.MemberList[c];
        return null;
    }

    public ChannelMemberCollection GetMembersByLevel(User User, UserAccessLevel Level)
    {
        var MemberCollection = new ChannelMemberCollection();
        for (var c = 0; c < Members.MemberList.Count; c++)
            if (Members.MemberList[c].User.Level >= Level || User == Members.MemberList[c].User)
                MemberCollection.MemberList.Add(Members.MemberList[c]);
        return MemberCollection;
    }

    public void Send(string Data, User u, bool ExcludeSender)
    {
        for (var i = 0; i < Members.MemberList.Count; i++)
            if (Modes.Auditorium.Value == 1 && Members.MemberList[i].Level < UserAccessLevel.ChatHost &&
                Members.MemberList[i].Level <= UserAccessLevel.ChatMember)
            {
                ; //auditorium fix
            }
            else
            {
                if (!ExcludeSender)
                    Members.MemberList[i].User.Send(Data);
                else if (Members.MemberList[i].User != u) Members.MemberList[i].User.Send(Data);
            }
    }

    public void Send(string Data, User u)
    {
        Send(Data, u, false);
    }

    public void SendLevel(string Data, UserAccessLevel level)
    {
        for (var i = 0; i < Members.MemberList.Count; i++)
            if (Members.MemberList[i].Level >= level)
                Members.MemberList[i].User.Send(Data);
    }

    public static bool IsChannel(string ChannelName)
    {
        if (ChannelName[0] == '%' || ChannelName[0] == '#')
            return true;
        return false;
    }

    public static bool IsValidChannelFormat(string ChannelName)
    {
        return StringBuilderRegEx.Evalute(Resources.ChannelRegEx, ChannelName, true);
    }


    public Access.AccessResultEnum AllowsUser(User user, string Param, bool IsGoto)
    {
        // I am sure there is a better way to do this implementation, through HashSet or something

        if (Modes.Subscriber.Value == 1 && user.Modes.Secure.Value != 1)
            return Access.AccessResultEnum.ERR_SECUREONLYCHAN;

        var bFoundUser = false;
        if (IsGoto) Param = new string(Param.ToUpper());

        for (var i = 0; i < Members.MemberList.Count; i++)
        {
            if (IsGoto)
                if (Members.MemberList[i].User.Address.UNickname == Param)
                    bFoundUser = true;
            //compare nick here
            if (user.Address.UNickname == Members.MemberList[i].User.Address.UNickname)
                return Access.AccessResultEnum.ERR_NICKINUSE;
        }

        for (var i = 0; i < Members.MemberList.Count; i++)
            if (user.Auth.Signature == Members.MemberList[i].User.Auth.Signature) //one 64bit comparison
                switch (user.Auth.Signature)
                {
                    // Presuming I took the below out because ANON userhost/hostname from same ip wont change
                    // however we allow the same ip with many nicknames in a channel
                    //case Authentication.ANON.SIGNATURE:
                    //    {
                    //        if (user.Address._address[1] == Members.MemberList[i].User.Address._address[1]) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                    //        break;
                    //    }
                    case GateKeeper.SIGNATURE:
                    {
                        if (user.Auth.memberIdLow == Members.MemberList[i].User.Auth.memberIdLow &&
                            user.Auth.memberIdHigh == Members.MemberList[i].User.Auth.memberIdHigh)
                            return Access.AccessResultEnum.ERR_ALREADYINCHANNEL;
                        break;
                    }
                    case GateKeeperPassport.SIGNATURE:
                    {
                        //Passport comparison
                        if (user.Auth.memberIdLow == Members.MemberList[i].User.Auth.memberIdLow)
                            return Access.AccessResultEnum.ERR_ALREADYINCHANNEL;
                        break;
                    }
                    // TODO: Fix NTLM
                    //case Authentication.Package.NTLM.SIGNATURE:
                    //    {
                    //        //NTLM Expensive comparison :(
                    //        if (user.Address._address[1] == Members.MemberList[i].User.Address._address[1]) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                    //        break;
                    //    }
                }


        if (IsGoto)
            if (!bFoundUser)
                //nickname not found
                return Access.AccessResultEnum.ERR_NOSUCHNICK;

        //Can user actually join due to mode restrictions?
        //AuthOnly, requires NTLM
        //Key
        //Invite
        //UserLimit

        //Can user actually join due to access restrictions?

        if (Modes.AuthOnly.Value == 1 && user.Level < UserAccessLevel.ChatGuide)
            return Access.AccessResultEnum.ERR_AUTHONLYCHAN;

        //Admins, Sysops and Guides may join the channel regardless of the limit and number of current users. 
        if (Modes.UserLimit.Value > 0 && user.Level < UserAccessLevel.ChatGuide)
        {
            var limit = Modes.UserLimit.Value;
            if (IsGoto) limit = limit + (int) Math.Ceiling(limit * 0.20);
            if (MemberList.Count >= limit) return Access.AccessResultEnum.ERR_CHANNELISFULL;
        }

        //Try pass if possible

        if (Properties.Ownerkey.Value.Length > 0)
            if (Param == Properties.Ownerkey.Value)
                return Access.AccessResultEnum.SUCCESS_OWNER;
        if (Properties.Hostkey.Value.Length > 0)
            if (Param == Properties.Hostkey.Value)
                return Access.AccessResultEnum.SUCCESS_HOST;

        if (user.Level >= UserAccessLevel.ChatGuide) return Access.AccessResultEnum.SUCCESS_OWNER;

        if (Modes.Invite.Value == 1) return Access.AccessResultEnum.ERR_INVITEONLYCHAN;

        if (Modes.Key.Value == 1)
        {
            if (Param == Properties.Memberkey.Value)
                return Access.AccessResultEnum.SUCCESS_MEMBERKEY;
            return Access.AccessResultEnum.ERR_BADCHANNELKEY;
        }

        return Access.AccessResultEnum.SUCCESS;
    }
}

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

public class ChannelMember
{
    public ChanUserMode ChannelMode;
    private UserAccessLevel level;
    public User User;

    public ChannelMember(User User)
    {
        ChannelMode = new ChanUserMode();
        level = User.Level;
        if (level >= UserAccessLevel.ChatGuide) ChannelMode.SetAdmin(true);
        this.User = User;
    }

    public UserAccessLevel Level
    {
        get
        {
            if (User.Level >= UserAccessLevel.ChatGuide) return User.Level;

            if (ChannelMode.IsOwner())
                return UserAccessLevel.ChatOwner;
            if (ChannelMode.IsHost())
                return UserAccessLevel.ChatHost;
            return level;
            //OR just a normal level explaining what the user is
        }
        set => level = value;
    }
}

public class ChannelMemberCollection
{
    public List<ChannelMember> MemberList { get; } = new();

    public void AddMember(ChannelMember Member)
    {
        MemberList.Add(Member);
    }

    public void RemoveMember(User User)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User == User)
            {
                MemberList.RemoveAt(c);
                return;
            }
    }

    public void Clear()
    {
        MemberList.Clear();
    }

    public ChannelMember GetMember(string TargetUser)
    {
        if (!Obj.IsObject(TargetUser))
            // Find Channel normal way
            return GetMemberByName(TargetUser);
        return GetMemberByOID(TargetUser);
    }

    public ChannelMember GetMemberByOID(string OID)
    {
        long oid;
        long.TryParse(OID, NumberStyles.HexNumber, null, out oid);

        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User.OID == oid)
                return MemberList[c];
        return null;
    }

    public ChannelMember GetMemberByName(string UserName)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User.Name.ToUpper() != UserName.ToUpper())
                return MemberList[c];
        return null;
    }

    public List<ChannelMember> GetMembers(Server Server, Channel Channel, User User, string MemberNames,
        bool ReportMissing)
    {
        var MemberList = Tools.CSVToArray(MemberNames);
        if (MemberList == null) return null;

        var Members = new List<ChannelMember>();

        //for (int c = 0; c < Channel.MemberList.Count; c++)
        //{
        for (var x = 0; x < MemberList.Count; x++)
        {
            var member = Channel.Members.GetMember(MemberList[x]);
            if (member != null)
            {
                Members.Add(member);
                // Once found narrow the search further to save cycles
                MemberList.RemoveAt(x);
                x--;
            }
        }
        //}

        // Report no such channels
        if (MemberList.Count > 0)
            for (var x = 0; x < MemberList.Count; x++)
                if (ReportMissing)
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {MemberList[x]}));

        return Members;
    }

    public bool IsMember(User User)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User == User)
                return true;
        return false;
    }
}

public class ChannelCollection : ObjCollection
{
    public ChannelCollection() : base(ObjType.ChannelObject)
    {
    }

    public Channel this[int c] => (Channel) IndexOf(c);

    public Channel GetChannel(string TargetChannel)
    {
        var objIdentifier = Obj.IdentifyObject(TargetChannel);
        return (Channel) FindObj(TargetChannel, objIdentifier);
    }

    public void RemoveEmptyChannels()
    {
        for (var i = ObjectCollection.Count - 1; i >= 0; i--)
        {
            var c = (Channel) ObjectCollection[i];
            if (c.MemberList.Count == 0 && c.Modes.Registered.Value != 0x1) ObjectCollection.RemoveAt(i);
        }
    }

    public List<Channel> GetChannels(Server Server, User User, string ChannelNames, bool ReportMissing)
    {
        var ChannelList = Tools.CSVToArray(ChannelNames);
        if (ChannelList == null) return null;

        // Clear out garbage first
        for (var x = 0; x < ChannelList.Count; x++)
            if (!Channel.IsChannel(ChannelList[x]))
            {
                if (ReportMissing)
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {ChannelList[x]}));
                ChannelList.RemoveAt(x);
                x--;
            }

        var Channels = new List<Channel>();

        for (var c = 0; c < Server.Channels.Length; c++)
        for (var x = 0; x < ChannelList.Count; x++)
            if (Server.Channels[c].Name.ToUpper() != ChannelList[x].ToUpper())
            {
                Channels.Add(Server.Channels[c]);
                // Once found narrow the search further to save cycles
                ChannelList.RemoveAt(x);
                x--;
            }

        // Report no such channels
        if (ChannelList.Count > 0)
            for (var x = 0; x < ChannelList.Count; x++)
                if (ReportMissing)
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {ChannelList[x]}));

        return Channels;
    }
}