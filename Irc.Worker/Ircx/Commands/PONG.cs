using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class PONG : Command
{
    public PONG(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        return true;
    }
}