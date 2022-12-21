using Irc.Commands;
using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Apollo.Directory.Commands;

internal class Ircvers : Command, ICommand
{
    public Ircvers() : base(2, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
    }
}