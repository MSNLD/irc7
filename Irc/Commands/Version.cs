using Irc.Enumerations;

namespace Irc.Commands;

public class Version : Command, ICommand
{
    public Version()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_VERSION_351(chatFrame.Server, chatFrame.User, chatFrame.Server.GetVersion()));
    }
}