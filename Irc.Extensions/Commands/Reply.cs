using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Reply : Command, ICommand
{
    public Reply()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
    }
}