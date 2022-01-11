using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class PONG : Command
{
    public PONG(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        return COM_RESULT.COM_SUCCESS;
    }
}