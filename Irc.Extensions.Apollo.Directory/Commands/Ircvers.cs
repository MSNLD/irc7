using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Ircvers : Command, ICommand
{
    public Ircvers() : base(2, false)
    {
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public void Execute(ChatFrame chatFrame)
    {
    }
}