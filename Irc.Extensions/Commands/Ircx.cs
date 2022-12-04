using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions;

namespace Irc.Commands;

internal class Ircx : Command, ICommand
{
    public Ircx() : base(0, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(ChatFrame chatFrame)
    {
        var protocol = chatFrame.User.GetProtocol().GetProtocolType();
        if (protocol < EnumProtocolType.IRCX)
        {
            protocol = EnumProtocolType.IRCX;
            chatFrame.User.SetProtocol(chatFrame.Server.GetProtocol(protocol));
        }

        var isircx = protocol > EnumProtocolType.IRC;
        chatFrame.User.GetModes().ToggleModeChar(ExtendedResources.UserModeIrcx, true);


        chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
            chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
    }
}