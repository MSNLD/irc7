using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Commands;

public class Pong : Command, ICommand
{
    public Pong() : base(0, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
    }
}