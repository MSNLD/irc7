using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security.Packages;
using Irc.Helpers.CSharpTools;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public class Channel : ChatObject
{
    public Access Access;
    public FloodProfile FloodProfile = new();
    public IList<User> InviteList = new List<User>();
    public IList<ChannelMember> Members = new List<ChannelMember>();
    public ChannelModeCollection Modes;
    public long TopicLastChanged;
    public long CreationDate;

    public Channel(string Name): base(new PropCollection())
    {
        Access = new Access(Name, true);
        Modes = new ChannelModeCollection();
        SetName(Name);

        foreach (string prop in ChannelProperties.PropertyRules.Keys)
        {
            Properties.Set(prop, null);
        }

        CreationDate = Common.GetCreationDate();
        Properties.Set(Resources.ChannelPropCreation, CreationDate.ToString());
        Properties.Set(Resources.ChannelPropLanguage, "1");
    }

    private void SetName(string Name)
    {
        this.Name = Name;
    }

    public ChannelMember GetMember(User User)
    {
        foreach (var channelMember in Members)
        {
            if (channelMember.User == User)
                return channelMember;
        }

        return null;
    }

    public IList<ChannelMember> GetMembersByLevel(User User, UserAccessLevel Level)
    {
        var MemberList = new List<ChannelMember>();
        foreach (var channelMember in Members)
        {
            if (channelMember.User.Level >= Level || User == channelMember.User)
                MemberList.Add(channelMember);
        }

        return MemberList;
    }

    public void Send(string Data, User u, bool ExcludeSender)
    {
        foreach (var channelMember in Members)
        {
            if (Modes.Auditorium.Value == 1 && channelMember.Level < UserAccessLevel.ChatHost &&
                channelMember.Level <= UserAccessLevel.ChatMember)
            {
                ; //auditorium fix
            }
            else
            {
                if (!ExcludeSender)
                    channelMember.User.Send(Data);
                else if (channelMember.User != u) channelMember.User.Send(Data);
            }
        }
    }

    public void Send(string Data, User u)
    {
        Send(Data, u, false);
    }

    public void SendLevel(string Data, UserAccessLevel level)
    {
        foreach (var channelMember in Members)
        {
            if (channelMember.Level >= level) channelMember.User.Send(Data);
        }
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


    public AccessResultEnum AllowsUser(User user, string Param, bool IsGoto)
    {
        // I am sure there is a better way to do this implementation, through HashSet or something

        if (Modes.Subscriber.Value == 1 && user.Modes.Secure.Value != 1)
            return AccessResultEnum.ERR_SECUREONLYCHAN;

        var bFoundUser = false;
        if (IsGoto) Param = new string(Param.ToUpper());

        foreach (var channelMember in Members)
        {
            if (IsGoto)
                if (channelMember.User.Address.Nickname.ToUpper() == Param)
                    bFoundUser = true;
            //compare nick here
            if (user.Address.Nickname.ToUpper() == channelMember.User.Address.Nickname.ToUpper())
                return AccessResultEnum.ERR_NICKINUSE;

            if (user.Address.GetUserHost() == channelMember.User.Address.GetUserHost())
            {
                return AccessResultEnum.ERR_ALREADYINCHANNEL;
            }
        }

        if (IsGoto)
            if (!bFoundUser)
                //nickname not found
                return AccessResultEnum.ERR_NOSUCHNICK;

        //Can user actually join due to mode restrictions?
        //AuthOnly, requires NTLM
        //Key
        //Invite
        //UserLimit

        //Can user actually join due to access restrictions?

        if (Modes.AuthOnly.Value == 1 && user.Level < UserAccessLevel.ChatGuide)
            return AccessResultEnum.ERR_AUTHONLYCHAN;

        //Admins, Sysops and Guides may join the channel regardless of the limit and number of current users. 
        if (Modes.UserLimit.Value > 0 && user.Level < UserAccessLevel.ChatGuide)
        {
            var limit = Modes.UserLimit.Value;
            if (IsGoto) limit = limit + (int) Math.Ceiling(limit * 0.20);
            if (Members.Count >= limit) return AccessResultEnum.ERR_CHANNELISFULL;
        }

        //Try pass if possible

        var ownerkey = Properties.Get("Ownerkey");
        if (!string.IsNullOrWhiteSpace(ownerkey))
            if (Param == ownerkey)
                return AccessResultEnum.SUCCESS_OWNER;

        var hostkey = Properties.Get("Hostkey");
        if (!string.IsNullOrWhiteSpace(hostkey))
            if (Param == hostkey)
                return AccessResultEnum.SUCCESS_HOST;

        if (user.Level >= UserAccessLevel.ChatGuide) return AccessResultEnum.SUCCESS_OWNER;

        if (Modes.Invite.Value == 1) return AccessResultEnum.ERR_INVITEONLYCHAN;

        if (Modes.Key.Value == 1)
        {
            var memberkey = Properties.Get("Memberkey");
            if (Param == memberkey)
                return AccessResultEnum.SUCCESS_MEMBERKEY;
            return AccessResultEnum.ERR_BADCHANNELKEY;
        }

        return AccessResultEnum.SUCCESS;
    }
}