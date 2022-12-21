using Irc.Commands;
using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Create : Command, ICommand
{
    public Create()
    {
        MinParams = 1;
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_FINDS_613(chatFrame.Server, chatFrame.User));
    }
}