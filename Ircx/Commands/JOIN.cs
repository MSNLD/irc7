using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;
using System.Text;

namespace Core.Ircx.Commands
{
    // NOTE: I commented out some feature code here from the looks of it

    public class JOIN : Command
    {
        public JOIN(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Join;
            base.ForceFloodCheck = true;
        }

        public static COM_RESULT ProcessJoinChannel(Frame Frame, Channel c, string Key)
        {
            Server server = Frame.Server;
            User user = Frame.User;
            Message message = Frame.Message;


            if (!user.IsOnChannel(c))
            {
                if ((user.Profile.Ircvers > 0) && (user.Profile.Ircvers < 9))
                {
                    if (user.ActiveChannel != null) { user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_TOOMANYCHANNELS_405)); return COM_RESULT.COM_SUCCESS; }
                }

                Access.AccessResultEnum AccessResult;

                //ACCESS check is performed here
                AccessResult = c.Access.GetAccess(user.Address).Result;

                Access.AccessResultEnum Allowed = c.AllowsUser(user, Key, false);

                if (Allowed == Access.AccessResultEnum.ERR_ALREADYINCHANNEL) { AccessResult = Allowed; }
                else if (Allowed == Access.AccessResultEnum.ERR_NICKINUSE) { AccessResult = Allowed; }
                else if (user.Level >= UserAccessLevel.ChatGuide) { AccessResult = Access.AccessResultEnum.SUCCESS_OWNER; } //IRCOp gets in regardless
                else if (Allowed == Access.AccessResultEnum.ERR_AUTHONLYCHAN) { AccessResult = Allowed; } // NTLM Only
                else if (AccessResult == Access.AccessResultEnum.ERR_BANNEDFROMCHAN) { Allowed = AccessResult; }
                else if ((Allowed == Access.AccessResultEnum.ERR_CHANNELISFULL) && (AccessResult > Access.AccessResultEnum.SUCCESS_HOST)) { AccessResult = Allowed; }
                else if ((Allowed == Access.AccessResultEnum.SUCCESS_MEMBERKEY) && (AccessResult > Access.AccessResultEnum.NONE)) { Allowed = AccessResult; } //Access overrides memberkey
                else if (AccessResult == Access.AccessResultEnum.NONE) { AccessResult = Allowed; }

                if ((Allowed <= Access.AccessResultEnum.SUCCESS_HOST) && (Allowed < AccessResult)) { AccessResult = Allowed; } //If ChanAccess is greater than Access then use that instead

                //if (c.IsInvited(user))
                //{
                //    if ((c.Modes.Invite.Value == 0x1) && (AccessResult == Access.AccessResultEnum.ERR_INVITEONLYCHAN))
                //    {
                //        AccessResult = Access.AccessResultEnum.SUCCESS;
                //    }
                //    if (AccessResult <= 0) { c.InviteList.Remove(user); } //remove user as it is a successful join
                //}

                ChannelMember Member = new ChannelMember(Frame.User);
                UserChannelInfo uci = new UserChannelInfo(c, Member);

                if (AccessResult <= 0)
                {
                    //user.ChannelMode = ChanUserMode.HOST;
                    //user.ChannelMode.SetOwner(true);

                    if (user.Level >= UserAccessLevel.ChatGuide) {
                        AccessResult = Access.AccessResultEnum.SUCCESS_OWNER;
                    } //global operators after access check
                    else if ((user.Level == UserAccessLevel.ChatHost) && (AccessResult < Access.AccessResultEnum.SUCCESS_HOST)) { AccessResult = Access.AccessResultEnum.SUCCESS_HOST; } //global hosts after access check

                    switch (AccessResult)
                    {
                        case Access.AccessResultEnum.SUCCESS_OWNER: { uci.Member.ChannelMode.SetOwner(true); break; }
                        case Access.AccessResultEnum.SUCCESS_HOST: { uci.Member.ChannelMode.SetHost(true); break; }
                        case Access.AccessResultEnum.SUCCESS_VOICE: { uci.Member.ChannelMode.SetVoice(true); break; }
                    }

                    c.AddMember(Member);
                    user.AddChannel(uci);

                    ProcessJoinToChannel(server, c, Member);

                    NAMES.ProcessNames(server, Member, c);
                    //Send Topic
                    TOPIC.SendTopicReply(server, user, c);

                    //ChannelPropOnJoin.ProcessMessage(server, c, user, c.Props.OnJoin);
                }
                else
                {
                    switch (AccessResult)
                    {
                        case Access.AccessResultEnum.ERR_NICKINUSE: { user.Send(Raws.Create(Server: server, Client: user, Raw: Raws.IRCX_ERR_NICKINUSE_433, Data: new string[] { user.Address.Nickname })); break; }
                        case Access.AccessResultEnum.ERR_ALREADYINCHANNEL: { user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_ALREADYONCHANNEL_927X)); break; }
                        case Access.AccessResultEnum.ERR_CHANNELISFULL:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_CHANNELISFULL_471));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw471); }
                                break;
                            }
                        case Access.AccessResultEnum.ERR_INVITEONLYCHAN:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_INVITEONLYCHAN_473));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw473); }
                                break;
                            }
                        case Access.AccessResultEnum.ERR_BANNEDFROMCHAN:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_BANNEDFROMCHAN_474));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw474); }
                                break;
                            }
                        case Access.AccessResultEnum.ERR_BADCHANNELKEY:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_BADCHANNELKEY_475));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw475); }
                                break;
                            }
                        case Access.AccessResultEnum.ERR_AUTHONLYCHAN:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_AUTHONLYCHAN_556));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw556); }
                                break;
                            }
                        case Access.AccessResultEnum.ERR_SECUREONLYCHAN:
                            {
                                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_SECUREONLYCHAN_557));
                                if (c.Modes.Knock.Value == 1) { SendKnock(server, c, user, Resources.Raw557); }
                                break;
                            }
                    }
                }
            }
            else { user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_ERR_ALREADYONCHANNEL_927X)); /* already in channel */ }

            return COM_RESULT.COM_SUCCESS;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Message message = Frame.Message;

            if (message.Data.Count >= 1)
            {
                // To process 0
                if ((message.Data[0] == Resources.Zero) && (message.Data.Count == 1)) { PART.ProcessPartUserChannels(Frame); return COM_RESULT.COM_SUCCESS; }

                List<string> Keys = new List<string>();
                string CurrentKey = Resources.Null;

                if (message.Data.Count > 1)
                {
                    Keys = CSharpTools.Tools.CSVToArray(message.Data[1], true);
                }

                List<Channel> channels = Frame.Server.Channels.GetChannels(Frame.Server, Frame.User, message.Data[0], true);

                if (channels != null)
                {
                    if (channels.Count > 0)
                    {
                        for (int c = 0; c < channels.Count; c++)
                        {
                            if (Keys.Count > c) { CurrentKey = Keys[c]; }
                            ProcessJoinChannel(Frame, channels[c], CurrentKey);
                        }
                        return COM_RESULT.COM_SUCCESS;
                    }
                }

                // null
                // No such channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new string[] { Frame.Message.Data[0] }));
            }
            else
            {
                //insufficient parameters
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461, Data: new string[] { message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }

        public static void SendKnock(Server server, Channel c, User from, string Reason)
        {
            string RawKnock = Raws.Create(Server: server, Channel: c, Client: from, Raw: Raws.RPL_KNOCK_CHAN, Data: new string[] { Reason });
            c.SendLevel(RawKnock, UserAccessLevel.ChatHost);
        }

        public static void ProcessJoinToChannel(Server server, Channel c, ChannelMember Member)
        {
            for (int i = 0; i < c.MemberList.Count; i++)
            {
                ChannelMember ChannelMember = c.MemberList[i];

                //if (((c.Modes.Auditorium.Value == 1) && (c.users[i].Level < UserAccessLevel.ChatHost) && (user.Level <= UserAccessLevel.ChatMember)) && (user != c.users[i])) ;
                //else
                //{

                    string UserProfile = Member.User.Profile.GetProfile(ChannelMember.User.Profile.Ircvers);
                    if (UserProfile.Length > 0)
                    {
                        //msn join
                        if (Member.ChannelMode.modeChar == 0x0)
                        {
                            ChannelMember.User.Send(Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.RPL_JOIN_MSN, Data: new string[] { UserProfile }));
                        }
                        else
                        {
                            StringBuilder ProfMode = new StringBuilder(2);
                            ProfMode.Append((char)44);
                            ProfMode.Append((char)Member.ChannelMode.modeChar);
                            ChannelMember.User.Send(Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.RPL_JOIN_MSN, Data: new string[] { UserProfile, ProfMode.ToString() }));
                        }
                    }
                    else
                    {
                        //normal join
                        ChannelMember.User.Send(Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.RPL_JOIN_IRC));
                        //raw +o or +q
                    }

                    if (ChannelMember.User.Profile.Ircvers <= 6)
                    {
                        if (!Member.ChannelMode.IsNormal())
                        {
                            StringBuilder ProfMode = new StringBuilder(3);
                            ProfMode.Append((char)43); // '+'
                            if (Member.ChannelMode.IsOwner()) { ProfMode.Append((char)Resources.ChannelUserModeCharOwner); }
                            else if (Member.ChannelMode.IsHost()) { ProfMode.Append((char)Resources.ChannelUserModeCharHost); }
                            else if (Member.ChannelMode.IsVoice()) { ProfMode.Append((char)Resources.ChannelUserModeCharVoice); }

                            if (ProfMode.Length == 2)
                            {
                                ProfMode.Append((char)0x20);
                                //itll only be 2 Length when it has passed above criteria
                                ChannelMember.User.Send(Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.RPL_MODE_IRC, Data: new string[] { c.Name, ProfMode.ToString(), Member.User.Address.Nickname }));
                            }
                        }
                    }

                //}
            }


        }
    }
}
