using System.Collections.Generic;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;
// NOTE: I commented out some feature code here from the looks of it

public class JOIN : Command
{
    public JOIN(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        RegistrationRequired = true;
        DataType = CommandDataType.Join;
        ForceFloodCheck = true;
    }

    public static bool ProcessJoinChannel(Frame Frame, Channel c, string Key)
    {
        var server = Frame.Server;
        var user = Frame.User;

        if (!user.Channels.ContainsKey(c))
        {
            if (user.Profile.Ircvers > 0 && user.Profile.Ircvers < 9)
                if (user.Channels.Count > 1)
                {
                    user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_TOOMANYCHANNELS_405));
                    return true;
                }

            AccessResultEnum AccessResult;

            //ACCESS check is performed here
            AccessResult = c.Access.GetAccess(user.Address).Result;

            var Allowed = c.AllowsUser(user, Key, false);

            if (Allowed == AccessResultEnum.ERR_ALREADYINCHANNEL)
                AccessResult = Allowed;
            else if (Allowed == AccessResultEnum.ERR_NICKINUSE)
                AccessResult = Allowed;
            else if (user.Level >= UserAccessLevel.ChatGuide)
                AccessResult = AccessResultEnum.SUCCESS_OWNER;
            else if (Allowed == AccessResultEnum.ERR_AUTHONLYCHAN)
                AccessResult = Allowed;
            else if (AccessResult == AccessResultEnum.ERR_BANNEDFROMCHAN)
                Allowed = AccessResult;
            else if (Allowed == AccessResultEnum.ERR_CHANNELISFULL &&
                     AccessResult > AccessResultEnum.SUCCESS_HOST)
                AccessResult = Allowed;
            else if (Allowed == AccessResultEnum.SUCCESS_MEMBERKEY &&
                     AccessResult > AccessResultEnum.NONE)
                Allowed = AccessResult;
            else if (AccessResult == AccessResultEnum.NONE) AccessResult = Allowed;

            if (Allowed <= AccessResultEnum.SUCCESS_HOST && Allowed < AccessResult) AccessResult = Allowed;

            //if (channel.IsInvited(user))
            //{
            //    if ((channel.Modes.Invite.Value == 0x1) && (AccessResult == AccessResultEnum.ERR_INVITEONLYCHAN))
            //    {
            //        AccessResult = AccessResultEnum.SUCCESS;
            //    }
            //    if (AccessResult <= 0) { channel.InviteList.Remove(user); } //remove user as it is a successful join
            //}

            var Member = new ChannelMember(Frame.User);

            if (AccessResult <= 0)
            {
                //user.ChannelMode = ChanUserMode.HOST;
                //user.ChannelMode.SetOwner(true);

                if (user.Level >= UserAccessLevel.ChatGuide)
                    AccessResult = AccessResultEnum.SUCCESS_OWNER;
                else if (user.Level == UserAccessLevel.ChatHost && AccessResult < AccessResultEnum.SUCCESS_HOST)
                    AccessResult = AccessResultEnum.SUCCESS_HOST;

                switch (AccessResult)
                {
                    case AccessResultEnum.SUCCESS_OWNER:
                    {
                        Member.ChannelMode.SetOwner(true);
                        break;
                    }
                    case AccessResultEnum.SUCCESS_HOST:
                    {
                        Member.ChannelMode.SetHost(true);
                        break;
                    }
                    case AccessResultEnum.SUCCESS_VOICE:
                    {
                        Member.ChannelMode.SetVoice(true);
                        break;
                    }
                }

                c.Members.Add(Member);
                user.AddChannel(c, Member);

                ProcessJoinToChannel(server, c, Member);

                NAMES.ProcessNames(server, Member, c);
                //Send Topic
                TOPIC.SendTopicReply(server, user, c);

                //ChannelPropOnJoin.ProcessMessage(server, channel, user, channel.Props.OnJoin);
            }
            else
            {
                switch (AccessResult)
                {
                    case AccessResultEnum.ERR_NICKINUSE:
                    {
                        user.Send(RawBuilder.Create(server, Client: user, Raw: Raws.IRCX_ERR_NICKINUSE_433,
                            Data: new[] {user.Address.Nickname}));
                        break;
                    }
                    case AccessResultEnum.ERR_ALREADYINCHANNEL:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_ALREADYONCHANNEL_927X));
                        break;
                    }
                    case AccessResultEnum.ERR_CHANNELISFULL:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_CHANNELISFULL_471));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw471);
                        break;
                    }
                    case AccessResultEnum.ERR_INVITEONLYCHAN:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_INVITEONLYCHAN_473));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw473);
                        break;
                    }
                    case AccessResultEnum.ERR_BANNEDFROMCHAN:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_BANNEDFROMCHAN_474));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw474);
                        break;
                    }
                    case AccessResultEnum.ERR_BADCHANNELKEY:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_BADCHANNELKEY_475));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw475);
                        break;
                    }
                    case AccessResultEnum.ERR_AUTHONLYCHAN:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_AUTHONLYCHAN_556));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw556);
                        break;
                    }
                    case AccessResultEnum.ERR_SECUREONLYCHAN:
                    {
                        user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_SECUREONLYCHAN_557));
                        if (c.Modes.Knock.Value == 1) SendKnock(server, c, user, Resources.Raw557);
                        break;
                    }
                }
            }
        }
        else
        {
            user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_ERR_ALREADYONCHANNEL_927X)); /* already in channel */
        }

        return true;
    }

    public new bool Execute(Frame Frame)
    {
        var message = Frame.Message;

        if (message.Parameters.Count >= 1)
        {
            // To process 0
            if (message.Parameters[0] == Resources.Zero && message.Parameters.Count == 1)
            {
                PART.ProcessPartUserChannels(Frame);
                return true;
            }

            var Keys = new List<string>();
            var CurrentKey = string.Empty;

            if (message.Parameters.Count > 1) Keys = Tools.CSVToArray(message.Parameters[1], true, Resources.MaxFieldLen);

            var channels = Common.GetChannels(Frame.Server, Frame.User, message.Parameters[0], true);

            if (channels != null)
                if (channels.Count > 0)
                {
                    for (var c = 0; c < channels.Count; c++)
                    {
                        if (Keys.Count > c) CurrentKey = Keys[c];
                        ProcessJoinChannel(Frame, channels[c], CurrentKey);
                    }

                    return true;
                }

            // null
            // No such channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                Data: new[] {Frame.Message.Parameters[0]}));
        }
        else
        {
            //insufficient parameters
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461,
                Data: new[] {message.Parameters[0]}));
        }

        return true;
    }

    public static void SendKnock(Server server, Channel c, User from, string Reason)
    {
        var RawKnock = RawBuilder.Create(server, c, from, Raws.RPL_KNOCK_CHAN, new[] {Reason});
        c.SendLevel(RawKnock, UserAccessLevel.ChatHost);
    }

    public static void ProcessJoinToChannel(Server server, Channel channel, ChannelMember Member)
    {
        foreach (var channelMember in channel.Members)
        {
            //if (((channel.Modes.Auditorium.Value == 1) && (channel.users[i].Level < UserAccessLevel.ChatHost) && (user.Level <= UserAccessLevel.ChatMember)) && (user != channel.users[i])) ;
            //else
            //{

            var UserProfile = Member.User.Profile.GetProfile(channelMember.User.Profile.Ircvers);
            if (UserProfile.Length > 0)
            {
                //msn join
                if (Member.ChannelMode.modeChar == 0x0)
                {
                    channelMember.User.Send(RawBuilder.Create(server, channel, Member.User, Raws.RPL_JOIN_MSN,
                        new[] {UserProfile}));
                }
                else
                {
                    var ProfMode = new StringBuilder(2);
                    ProfMode.Append((char) 44);
                    ProfMode.Append((char) Member.ChannelMode.modeChar);
                    channelMember.User.Send(RawBuilder.Create(server, channel, Member.User, Raws.RPL_JOIN_MSN,
                        new[] {UserProfile, ProfMode.ToString()}));
                }
            }
            else
            {
                //normal join
                channelMember.User.Send(RawBuilder.Create(server, channel, Member.User, Raws.RPL_JOIN_IRC));
                //raw +o or +q
            }

            if (channelMember.User.Profile.Ircvers <= 6)
                if (!Member.ChannelMode.IsNormal())
                {
                    var ProfMode = new StringBuilder(3);
                    ProfMode.Append((char) 43); // '+'
                    if (Member.ChannelMode.IsOwner())
                        ProfMode.Append((char) Resources.ChannelUserModeCharOwner);
                    else if (Member.ChannelMode.IsHost())
                        ProfMode.Append((char) Resources.ChannelUserModeCharHost);
                    else if (Member.ChannelMode.IsVoice()) ProfMode.Append((char) Resources.ChannelUserModeCharVoice);

                    if (ProfMode.Length == 2)
                    {
                        ProfMode.Append((char) 0x20);
                        //itll only be 2 Length when it has passed above criteria
                        channelMember.User.Send(RawBuilder.Create(server, channel, Member.User, Raws.RPL_MODE_IRC,
                            new[] {channelMember.User.Name, ProfMode.ToString(), Member.User.Address.Nickname}));
                    }
                }

            //}
        }
    }
}