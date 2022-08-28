using Irc.Enumerations;

namespace Irc.Commands;

internal class Privmsg : Command, ICommand
{
    public Privmsg() : base(2) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

    public new void Execute(ChatFrame chatFrame)
    {
        SendMessage(chatFrame, false);
    }

    public static void SendMessage(ChatFrame chatFrame, bool Notice)
    {
        var targetName = chatFrame.Message.Parameters.First();
        var message = chatFrame.Message.Parameters[1];

        string[] targets = targetName.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var target in targets)
        {
            Interfaces.IChatObject chatObject = null;
            if (Objects.Channel.Channel.ValidName(target))
            {
                chatObject = (Interfaces.IChatObject)chatFrame.Server.GetChannelByName(target);
            }
            else
            {
                chatObject = (Interfaces.IChatObject)chatFrame.Server.GetUserByNickname(target);
            }

            if (chatObject == null)
            {
                // TODO: To make common function for this
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, target));
                return;
            }

            if (chatObject is Objects.Channel.Channel)
            {
                if (Notice) ((Objects.Channel.Channel)chatObject).SendNotice(chatFrame.User, message);
                else ((Objects.Channel.Channel)chatObject).SendMessage(chatFrame.User, message);
            }
            else if (chatObject is Objects.User)
            {
                if (Notice)
                {
                    ((Objects.User)chatObject).Send(
                            Raw.RPL_NOTICE_USER(chatFrame.Server, chatFrame.User, chatObject, message)
                        );
                }
                else
                {
                    ((Objects.User)chatObject).Send(
                            Raw.RPL_PRIVMSG_USER(chatFrame.Server, chatFrame.User, chatObject, message)
                        );
                }

            }
        }
    }
}