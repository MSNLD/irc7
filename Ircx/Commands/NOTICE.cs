using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

public class NOTICE : Command
{
    public NOTICE(CommandCode Code) : base(Code)
    {
        MinParamCount = 2;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        return PRIVMSG.ProcessMessage(Frame, false);
    }
}