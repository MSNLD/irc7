using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Extensions.Commands;

internal class Data : Command, ICommand
{
    public Data() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Access)));
    }
}