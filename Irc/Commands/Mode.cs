using Irc.Constants;
using Irc.Enumerations;

namespace Irc.Commands;

internal class Mode : Command, ICommand
{
    public Mode()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        if (!chatFrame.User.Registered)
        {
            if (chatFrame.Message.Parameters.First().ToUpper() == Resources.ISIRCX)
            {
                var protocol = chatFrame.User.GetProtocol().GetProtocolType();
                var isircx = protocol > EnumProtocolType.IRC;
                chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
                    chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
            }
        }
        else
        {
            // TODO: implement MODE
            var channelName = chatFrame.Message.Parameters.First();
            var channel = chatFrame.Server.GetChannelByName(channelName);
            chatFrame.User.Send(Raw.IRCX_RPL_MODE_324(chatFrame.Server, chatFrame.User, channel, channel.GetModes().ToString()));
        }
    }
}