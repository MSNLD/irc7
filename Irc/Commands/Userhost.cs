using Irc.Enumerations;

namespace Irc.Commands;

internal class Userhost : Command, ICommand
{
    public Userhost() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        if (chatFrame.User.IsRegistered())
            chatFrame.User.Send(Raw.IRCX_RPL_USERHOST_302(chatFrame.Server, chatFrame.User));
        // TODO: What if not registered?
    }
}