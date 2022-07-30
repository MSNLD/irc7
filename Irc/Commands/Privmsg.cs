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
        var channelName = chatFrame.Message.Parameters.First();
        var message = chatFrame.Message.Parameters[1];

        var channel = chatFrame.Server.GetChannelByName(channelName);

        if (channel != null)
            if (Notice) channel.SendNotice(chatFrame.User, message);
            else channel.SendMessage(chatFrame.User, message);
        else
            // TODO: To make common function for this
            chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
    }
}