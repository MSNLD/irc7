using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Directory;
using Irc.Interfaces;

namespace Irc.Extensions.Apollo.Commands;

internal class Finds : Command, ICommand
{
    public Finds()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send(ApolloDirectoryRaws.RPL_FINDS_MSN((DirectoryServer)chatFrame.Server, chatFrame.User));
    }
}