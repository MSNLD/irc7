using Irc.Enumerations;

namespace Irc.Commands;

internal class Pong : Command, ICommand
{
    public Pong() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame) { }
}