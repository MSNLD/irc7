using Irc.Enumerations;

namespace Irc.Commands;

internal class Userhost : Command, ICommand
{
    public Userhost()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        if (chatFrame.User.IsRegistered())
            chatFrame.User.Send(Raw.IRCX_RPL_USERHOST_302(chatFrame.Server, chatFrame.User));
        // TODO: What if not registered?
    }
}