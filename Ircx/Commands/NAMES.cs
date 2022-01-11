using System.Text;
using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

public class NAMES : Command
{
    public NAMES(CommandCode Code) : base(Code)
    {
        MinParamCount = 1; // to suppress any warnings
        RegistrationRequired = true;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        var Channel = Frame.Server.Channels.GetChannel(Frame.Message.Data[0]);
        if (Channel != null)
        {
            var Member = Channel.GetMember(Frame.User);
            if (Member != null)
                ProcessNames(Frame.Server, Member, Channel);
            else
                // you are not on that channel
                Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                    Data: new[] {Frame.Message.Data[0]}));
        }
        else
        {
            //no such channel
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                Data: new[] {Frame.Message.Data[0]}));
        }

        return COM_RESULT.COM_SUCCESS;
    }

    public static void ProcessNames(Server server, ChannelMember Member, Channel c)
    {
        var Names = new StringBuilder(512);
        var NameReply = Raws.Create(server, c, Member.User, Raws.IRCX_RPL_NAMEREPLY_353X, Newline: false);

        Names.Append(NameReply);
        for (var i = 0; i < c.MemberList.Count; i++)
            if (c.Modes.Auditorium.Value == 1 && c.MemberList[i].Level < UserAccessLevel.ChatHost &&
                Member.Level <= UserAccessLevel.ChatMember && Member != c.MemberList[i])
            {
                ;
            }
            else
            {
                string Nickname = c.MemberList[i].User.Address.Nickname,
                    PassportProf = c.MemberList[i].User.Profile.GetProfile(Member.User.Profile.Ircvers);
                var expectedLength = Nickname.Length + PassportProf.Length + (Member.User.Profile.Ircvers > 3 ? 1 : 0) +
                                     (Member.ChannelMode.UserMode != ChanUserMode.Normal ? 1 : 0);

                if (Names.Length + expectedLength < 510)
                {
                    if (Member.User.Profile.Ircvers > 3)
                    {
                        Names.Append(PassportProf);
                        Names.Append((char) 44);
                    }

                    if (c.MemberList[i].ChannelMode.IsHost() || c.MemberList[i].ChannelMode.IsOwner())
                    {
                        Names.Append((char) c.MemberList[i].ChannelMode.modeChar);

                        // If the member is host or owner, the + voice flag is suffixed after the . or @
                        if (c.MemberList[i].ChannelMode.UserMode > ChanUserMode.Voice)
                            if (c.MemberList[i].ChannelMode.IsVoice())
                                // however only under non-ircvers, irc0 and irc9
                                switch (Member.User.Profile.Ircvers)
                                {
                                    case -1:
                                    case 0:
                                    case 9:
                                    {
                                        Names.Append(Resources.FlagVoice);
                                        break;
                                    }
                                }
                    }
                    else if (c.MemberList[i].ChannelMode.IsVoice())
                    {
                        Names.Append((char) c.MemberList[i].ChannelMode.modeChar);
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

        Names.Length--; //to get rid of tailing space
        Names.Append(Resources.CRLF);
        Member.User.Send(new string(Names.ToString()));
        Member.User.Send(Raws.Create(server, Client: Member.User, Raw: Raws.IRCX_RPL_ENDOFNAMES_366,
            Data: new[] {c.Name}));
    }
}