using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Ison : Command, ICommand
{

    // TODO: For Apollo ISON is now a SysOp only command
    // For non-SysOp users an empty list is returned with RPL_ISON.
    public Ison()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Ison)));
    }
}