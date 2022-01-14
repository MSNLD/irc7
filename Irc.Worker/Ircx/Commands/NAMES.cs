using System.Text;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class NAMES : Command
{
    public NAMES(CommandCode Code) : base(Code)
    {
        MinParamCount = 1; // to suppress any warnings
        RegistrationRequired = true;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        var objType = IrcHelper.IdentifyObject(Frame.Message.Parameters[0]);
        var Channel = Frame.Server.Channels.FindObj(Frame.Message.Parameters[0], objType);
        if (Channel != null)
        {
            var Member = Channel.GetMember(Frame.User);
            if (Member != null)
                ProcessNames(Frame.Server, Member, Channel);
            else
                // you are not on that channel
                Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                    Data: new[] {Frame.Message.Parameters[0]}));
        }
        else
        {
            //no such channel
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403,
                Data: new[] {Frame.Message.Parameters[0]}));
        }

        return true;
    }

    public static void ProcessNames(Server server, ChannelMember Member, Channel c)
    {
        var Names = new StringBuilder(512);
        var NameReply = RawBuilder.Create(server, c, Member.User, Raws.IRCX_RPL_NAMEREPLY_353X, Newline: false);

        Names.Append(NameReply);
        foreach (var channelMember in c.Members)
            if (c.Modes.Auditorium.Value == 1 && channelMember.Level < UserAccessLevel.ChatHost &&
                Member.Level <= UserAccessLevel.ChatMember && Member != channelMember)
            {
                ;
            }
            else
            {
                string Nickname = channelMember.User.Address.Nickname,
                    PassportProf = channelMember.User.Profile.GetProfile(Member.User.Profile.Ircvers);
                var expectedLength = Nickname.Length + PassportProf.Length + (Member.User.Profile.Ircvers > 3 ? 1 : 0) +
                                     (Member.ChannelMode.UserMode != ChanUserMode.Normal ? 1 : 0);

                if (Names.Length + expectedLength < 510)
                {
                    if (Member.User.Profile.Ircvers > 3)
                    {
                        Names.Append(PassportProf);
                        Names.Append((char) 44);
                    }

                    if (channelMember.ChannelMode.IsHost() || channelMember.ChannelMode.IsOwner())
                    {
                        Names.Append((char)channelMember.ChannelMode.modeChar);

                        // If the member is host or owner, the + voice flag is suffixed after the . or @
                        if (channelMember.ChannelMode.UserMode > ChanUserMode.Voice)
                            if (channelMember.ChannelMode.IsVoice())
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
                    else if (channelMember.ChannelMode.IsVoice())
                    {
                        Names.Append((char)channelMember.ChannelMode.modeChar);
                    }

                    Names.Append(channelMember.User.Address.Nickname);
                    Names.Append(' ');
                }
                else
                {
                    Names.Length--; //to get rid of tailing space
                    Names.Append(Resources.CRLF);
                    Member.User.Send(new string(Names.ToString()));
                    Names.Length = 0;
                    Names.Append(NameReply);
                }
            }

        Names.Length--; //to get rid of tailing space
        Names.Append(Resources.CRLF);
        Member.User.Send(new string(Names.ToString()));
        Member.User.Send(RawBuilder.Create(server, Client: Member.User, Raw: Raws.IRCX_RPL_ENDOFNAMES_366,
            Data: new[] {c.Name}));
    }
}