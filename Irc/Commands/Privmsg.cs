using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Objects.Channel;
using Irc.Objects.User;

namespace Irc.Commands;

public class Privmsg : Command, ICommand
{
    public Privmsg() : base(2)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        SendMessage(chatFrame, false);
    }

    public static void SendMessage(IChatFrame chatFrame, bool notice)
    {
        var targetName = chatFrame.Message.Parameters.First();
        var message = chatFrame.Message.Parameters[1];

        var targets = targetName.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var target in targets)
        {
            IChatObject chatObject = null;
            if (Channel.ValidName(target))
                chatObject = chatFrame.Server.GetChannelByName(target);
            else
                chatObject = chatFrame.Server.GetUserByNickname(target);

            if (chatObject == null)
            {
                // TODO: To make common function for this
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, target));
                return;
            }

            if (chatObject is Channel)
            {
                if (notice) ((Channel)chatObject).SendNotice(chatFrame.User, message);
                else ((Channel)chatObject).SendMessage(chatFrame.User, message);
            }
            else if (chatObject is User)
            {
                if (notice)
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