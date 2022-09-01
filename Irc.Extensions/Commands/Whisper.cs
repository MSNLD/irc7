using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Extensions.Commands;

internal class Whisper : Command, ICommand
{
    public Whisper() : base(3) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        // <sender> WHISPER <channel> <nick list> :<text>

        var channel = chatFrame.Server.GetChannelByName(chatFrame.Message.Parameters.First());

        if (channel != null) {
            var target = channel.GetMemberByNickname(chatFrame.Message.Parameters[1]);
            if (target != null) {
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
            else {
                // No such nickname
            }
        }
        else {
            // No such channel
        }

    }
}