using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class ISIRCX : Command
{
    public ISIRCX(CommandCode Code) : base(Code)
    {
        RegistrationRequired = false;
        DataType = CommandDataType.Standard;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        IRCX.ProcessIRCXReply(Frame);
        return COM_RESULT.COM_SUCCESS;
    }
}