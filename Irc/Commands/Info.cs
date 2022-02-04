using Irc.Enumerations;

namespace Irc.Commands;

internal class Info : Command, ICommand
{
    public Info()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_VERS(chatFrame.Server, chatFrame.User, chatFrame.Server.GetVersion()));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_RUNAS(chatFrame.Server, chatFrame.User));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_UPTIME(chatFrame.Server, chatFrame.User, chatFrame.Server.GetDataStore().GetAs<DateTime>("creation")));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_ENDOFINFO_374(chatFrame.Server, chatFrame.User));
    }
}