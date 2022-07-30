using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Ison : Command, ICommand
{
    // TODO: For Apollo ISON is now a SysOp only command
    // For non-SysOp users an empty list is returned with RPL_ISON.
    public Ison() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Ison)));
    }
}