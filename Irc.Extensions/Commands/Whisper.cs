using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Extensions.Commands;

internal class Whisper : Command, ICommand
{
    public Whisper() : base(1, true, 3) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        // <sender> WHISPER <channel> <nick list> :<text>

        var server = chatFrame.Server;
        var user = chatFrame.User;

        if (chatFrame.Message.Parameters.Count == 1)
        {
            user.Send(Raw.IRC_ERR_NORECIPIENT_411(server, user, typeof(Whisper).Name));
            return;
        }

        if (chatFrame.Message.Parameters.Count == 2)
        {
            user.Send(Raw.IRC_ERR_NOTEXT_412(server, user, typeof(Whisper).Name));
            return;
        } 
        
        var channelName = chatFrame.Message.Parameters.First();
        var channel = chatFrame.Server.GetChannelByName(channelName);
        if (channel == null)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, channelName));
            return;
        }

        var targetNickname = chatFrame.Message.Parameters[1];
        var target = channel.GetMemberByNickname(targetNickname);
        if (target == null)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHNICK_401(server, user, targetNickname));
            return;
        }

        var message = chatFrame.Message.Parameters[2];

        if (target.GetUser().GetProtocol().GetProtocolType() < EnumProtocolType.IRCX) {
            // PRIVMSG
            target.GetUser().Send(
                Raw.RPL_PRIVMSG_USER(chatFrame.Server, chatFrame.User, (ChatObject)target.GetUser(), message)
            );
        }
        else {
            target.GetUser().Send(
                Raw.RPL_CHAN_WHISPER(chatFrame.Server, chatFrame.User, channel, (ChatObject)target.GetUser(), message)
            );
        }

    }
}