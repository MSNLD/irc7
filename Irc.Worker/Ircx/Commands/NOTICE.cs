using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class NOTICE : Command
{
    public NOTICE(CommandCode Code) : base(Code)
    {
        MinParamCount = 2;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
    }

    public new bool Execute(Frame Frame)
    {
        return PRIVMSG.ProcessMessage(Frame, false);
    }
}