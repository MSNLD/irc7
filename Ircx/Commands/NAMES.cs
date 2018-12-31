using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class NAMES : Command
    {
        public NAMES(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1; // to suppress any warnings
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Channel Channel = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
            if (Channel != null)
            {
                ChannelMember Member = Channel.GetMember(Frame.User);
                if (Member != null)
                {
                    ProcessNames(Frame.Server, Member, Channel);
                }
                else
                {
                    // you are not on that channel
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new String8[] { Frame.Message.Data[0] }));
                }
            }
            else
            {
                //no such channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new String8[] { Frame.Message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
        public static void ProcessNames(Server server, ChannelMember Member, Channel c)
        {
            String8 Names = new String8(512);
            String8 NameReply = Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.IRCX_RPL_NAMEREPLY_353X, Newline: false);

            Names.append(NameReply);
            for (int i = 0; i < c.MemberList.Count; i++)
            {
                if (((c.Modes.Auditorium.Value == 1) && (c.MemberList[i].Level < UserAccessLevel.ChatHost) && (Member.Level <= UserAccessLevel.ChatMember)) && (Member != c.MemberList[i])) ;
                else
                {
                    String8 Nickname = c.MemberList[i].User.Address.Nickname,
                    PassportProf = c.MemberList[i].User.Profile.GetProfile(Member.User.Profile.Ircvers);
                    int expectedLength = Nickname.length + PassportProf.length + (Member.User.Profile.Ircvers > 3 ? 1 : 0) + (Member.ChannelMode.UserMode != ChanUserMode.Normal ? 1 : 0);

                    if (Names.length + expectedLength < 510)
                    {
                        if (Member.User.Profile.Ircvers > 3)
                        {
                            Names.append(PassportProf);
                            Names.append(44);
                        }
                        if ((c.MemberList[i].ChannelMode.IsHost()) || (c.MemberList[i].ChannelMode.IsOwner()))
                        {
                            Names.append((byte)c.MemberList[i].ChannelMode.modeChar);

                            // If the member is host or owner, the + voice flag is suffixed after the . or @
                            if (c.MemberList[i].ChannelMode.UserMode > ChanUserMode.Voice)
                            {
                                if (c.MemberList[i].ChannelMode.IsVoice())
                                {
                                    // however only under non-ircvers, irc0 and irc9
                                    switch (Member.User.Profile.Ircvers)
                                    {
                                        case -1: case 0: case 9: { Names.append(Resources.FlagVoice); break; }
                                    }
                                }
                            }
                        }
                        else if (c.MemberList[i].ChannelMode.IsVoice())
                        {
                            Names.append((byte)c.MemberList[i].ChannelMode.modeChar);
                        }

                        Names.append(c.MemberList[i].User.Address.Nickname);
                        Names.append(' ');
                    }
                    else
                    {
                        Names.length--; //to get rid of tailing space
                        Names.append(Resources.CRLF);
                        Member.User.Send(new String8(Names.bytes, 0, Names.length));
                        Names.length = 0;
                        Names.append(NameReply);
                        i--;
                    }
                }
            }
            Names.length--; //to get rid of tailing space
            Names.append(Resources.CRLF);
            Member.User.Send(new String8(Names.bytes, 0, Names.length));
            Member.User.Send(Raws.Create(Server: server, Client: Member.User, Raw: Raws.IRCX_RPL_ENDOFNAMES_366, Data: new String8[] { c.Name }));
        }
    }
}
