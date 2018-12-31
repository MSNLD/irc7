using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class KICK : Command
    {

        public KICK(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 2;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data.Count >= 2)
            {
                //kick # nick,[...] :reason
                Channel c = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
                if (c != null)
                {
                    UserChannelInfo uci = Frame.User.GetChannelInfo(c);
                    if (uci != null)
                    {
                        if (uci.Member.Level >= UserAccessLevel.ChatHost)
                        {
                            List<ChannelMember> Members = c.Members.GetMembers(Frame.Server, c, Frame.User, Frame.Message.Data[1], true);

                            if (Members != null) {
                                if (Members.Count > 0)
                                {
                                    String8 Reason = Resources.Null;
                                    if (Frame.Message.Data.Count >= 3) { Reason = Frame.Message.Data[2]; }

                                    for (int x = 0; x < Members.Count; x++)
                                    {
                                        ProcessKick(Frame.Server, uci.Member, c, Members[x], Reason);
                                    }
                                }
                                else
                                {
                                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new String8[] { Resources.Null }));
                                }
                            }
                            else
                            {
                                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new String8[] { Resources.Null }));
                            }

                        }
                        else
                        {
                            //not an operator
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: c, Client: Frame.User, Raw: Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                        }
                    }
                    else
                    {
                        //you're not on that channel
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new String8[] { Frame.Message.Data[0] }));
                    }
                }
                else
                {
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new String8[] { Frame.Message.Data[0] }));
                    //no such channel
                }
            }
            else
            {
                //insufficient parameters
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461, Data: new String8[] { Frame.Message.Command }));
            }
            return COM_RESULT.COM_SUCCESS;
        }

        public void ProcessKick(Server server, ChannelMember Member, Channel channel, ChannelMember ChannelMember, String8 Reason)
        {
            if (Member.Level < ChannelMember.Level)
            {
                if (ChannelMember.Level >= UserAccessLevel.ChatGuide) { /* you're not ircop */ Member.User.Send(Raws.Create(Server: server, Client: Member.User, Raw: Raws.IRCX_ERR_NOPRIVILEGES_481)); }
                else { /* you're not an operator 482 */ Member.User.Send(Raws.Create(Server: server, Channel: channel, Client: Member.User, Raw: Raws.IRCX_ERR_CHANOPRIVSNEEDED_482)); }
            }
            else
            {
                channel.Send(Raws.Create(Server: server, Channel: channel, Client: Member.User, Raw: Raws.RPL_KICK_IRC, Data: new String8[] { ChannelMember.User.Address.Nickname, Reason }), ChannelMember.User);
                ChannelMember.ChannelMode.SetNormal();
                channel.RemoveMember(ChannelMember);
                ChannelMember.User.RemoveChannel(channel);
            }
        }
    }
}
