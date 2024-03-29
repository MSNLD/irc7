using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Interfaces;
using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Channel;

namespace Irc.Extensions.Apollo.Commands;

using IrcPrivmsg = global::Irc.Commands.Privmsg;

public class Privmsg : IrcPrivmsg, ICommand
{
    public new void Execute(IChatFrame chatFrame)
    {
        SendMessage(chatFrame, false);
    }

    // TODO: Refactor this as it duplicates Privmsg
    public static void SendMessage(IChatFrame chatFrame, bool Notice)
    {
        var targetName = chatFrame.Message.Parameters.First();
        var message = chatFrame.Message.Parameters[1];

        var targets = targetName.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var target in targets)
        {
            IChatObject chatObject = null;
            if (Channel.ValidName(target))
                chatObject = (IChatObject)chatFrame.Server.GetChannelByName(target);
            else
                chatObject = (IChatObject)chatFrame.Server.GetUserByNickname(target);

            if (chatObject == null)
            {
                // TODO: To make common function for this
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, target));
                return;
            }

            if (chatObject is Channel)
            {
                var user = (ApolloUser)chatFrame.User;
                var channel = (ApolloChannel)chatObject;
                var channelModes = (IApolloChannelModes)channel.GetModes();
                var channelMember = channel.GetMember(chatFrame.User);
                var isOnChannel = channelMember != null;
                var noExtern = channelModes.NoExtern;
                var moderated = channelModes.Moderated;
                var subscriberOnly = channelModes.Subscriber;

                // Cannot send as a non-subscriber
                if (user.GetLevel() < EnumUserAccessLevel.Guide &&
                    !user.GetProfile().IsSubscriber &&
                    subscriberOnly
                   )
                {
                    chatFrame.User.Send(
                        Raw.IRCX_ERR_CANNOTSENDTOCHAN_404(chatFrame.Server, chatFrame.User, channel));
                    return;
                }

                if (
                    // No External Messages
                    (!isOnChannel && noExtern) ||
                    // Moderated
                    (isOnChannel && moderated && channelMember.IsNormal())
                )
                {
                    chatFrame.User.Send(
                        Raw.IRCX_ERR_CANNOTSENDTOCHAN_404(chatFrame.Server, chatFrame.User, channel));
                    return;
                }

                if (Notice) ((Channel)chatObject).SendNotice(chatFrame.User, message);
                else ((Channel)chatObject).SendMessage(chatFrame.User, message);
            }
            else if (chatObject is User)
            {
                if (Notice)
                    ((User)chatObject).Send(
                        Raw.RPL_NOTICE_USER(chatFrame.Server, chatFrame.User, chatObject, message)
                    );
                else
                    ((User)chatObject).Send(
                        Raw.RPL_PRIVMSG_USER(chatFrame.Server, chatFrame.User, chatObject, message)
                    );
            }
        }
    }
}