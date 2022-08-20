using Irc.Commands;
using Irc.Constants;
using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Ircvers : Command, ICommand
{
    public Ircvers()
    {
        _requiredMinimumParameters = 2;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public void Execute(ChatFrame chatFrame)
    {

    }
}