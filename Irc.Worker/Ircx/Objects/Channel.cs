using System;
using System.Collections.Generic;
using System.Reflection;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Access;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security.Packages;

namespace Irc.Worker.Ircx.Objects;

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

        foreach (var member in Members.MemberList)
        {
            if (user.Address._address[1] == member.User.Address._address[1])
            {
                return Access.AccessResultEnum.ERR_ALREADYINCHANNEL;
            }
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