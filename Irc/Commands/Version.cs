using Irc.Enumerations;

namespace Irc.Commands;

public class Version : Command, ICommand
{
    public Version() : base(0, false) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_VERSION_351(chatFrame.Server, chatFrame.User, chatFrame.Server.ServerVersion));
    }
}