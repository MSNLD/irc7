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
                    Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new string[] { Frame.Message.Data[0] }));
                }
            }
            else
            {
                //no such channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new string[] { Frame.Message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
        public static void ProcessNames(Server server, ChannelMember Member, Channel c)
        {
            StringBuilder Names = new StringBuilder(512);
            string NameReply = Raws.Create(Server: server, Channel: c, Client: Member.User, Raw: Raws.IRCX_RPL_NAMEREPLY_353X, Newline: false);

            Names.Append(NameReply);
            for (int i = 0; i < c.MemberList.Count; i++)
            {
                if (((c.Modes.Auditorium.Value == 1) && (c.MemberList[i].Level < UserAccessLevel.ChatHost) && (Member.Level <= UserAccessLevel.ChatMember)) && (Member != c.MemberList[i])) ;
                else
                {
                    string Nickname = c.MemberList[i].User.Address.Nickname,
                    PassportProf = c.MemberList[i].User.Profile.GetProfile(Member.User.Profile.Ircvers);
                    int expectedLength = Nickname.Length + PassportProf.Length + (Member.User.Profile.Ircvers > 3 ? 1 : 0) + (Member.ChannelMode.UserMode != ChanUserMode.Normal ? 1 : 0);

                    if (Names.Length + expectedLength < 510)
                    {
                        if (Member.User.Profile.Ircvers > 3)
                        {
                            Names.Append(PassportProf);
                            Names.Append((char)44);
                        }
                        if ((c.MemberList[i].ChannelMode.IsHost()) || (c.MemberList[i].ChannelMode.IsOwner()))
                        {
                            Names.Append((char)c.MemberList[i].ChannelMode.modeChar);

                            // If the member is host or owner, the + voice flag is suffixed after the . or @
                            if (c.MemberList[i].ChannelMode.UserMode > ChanUserMode.Voice)
                            {
                                if (c.MemberList[i].ChannelMode.IsVoice())
                                {
                                    // however only under non-ircvers, irc0 and irc9
                                    switch (Member.User.Profile.Ircvers)
                                    {
                                        case -1: case 0: case 9: { Names.Append(Resources.FlagVoice); break; }
                                    }
                                }
                            }
                        }
                        else if (c.MemberList[i].ChannelMode.IsVoice())
                        {
                            Names.Append((char)c.MemberList[i].ChannelMode.modeChar);
                        }

                        Names.Append(c.MemberList[i].User.Address.Nickname);
                        Names.Append(' ');
                    }
                    else
                    {
                        Names.Length--; //to get rid of tailing space
                        Names.Append(Resources.CRLF);
                        Member.User.Send(new string(Names.ToString()));
                        Names.Length = 0;
                        Names.Append(NameReply);
                        i--;
                    }
                }
            }
            Names.Length--; //to get rid of tailing space
            Names.Append(Resources.CRLF);
            Member.User.Send(new string(Names.ToString()));
            Member.User.Send(Raws.Create(Server: server, Client: Member.User, Raw: Raws.IRCX_RPL_ENDOFNAMES_366, Data: new string[] { c.Name }));
        }
    }
}
