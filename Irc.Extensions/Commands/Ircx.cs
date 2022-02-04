using Irc.Constants;
using Irc.Enumerations;

namespace Irc.Commands;

internal class Ircx : Command, ICommand
{
    public Ircx()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        var protocol = chatFrame.User.GetProtocol().GetProtocolType();
        if (protocol < EnumProtocolType.IRC0)
        {
            protocol = EnumProtocolType.IRCX;
            chatFrame.User.SetProtocol(chatFrame.Server.GetProtocol(protocol));
        }

        var isircx = protocol > EnumProtocolType.IRC;

        chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
            chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
    }
}