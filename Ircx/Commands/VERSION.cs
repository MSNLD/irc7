using Core.Ircx.Objects;

namespace Core.Ircx.Commands;

internal class VERSION : Command
{
    public VERSION(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_VERSION_351,
            IData: new[]
            {
                Program.Config.major, Program.Config.minor, Program.Config.build, Program.Config.major,
                Program.Config.minor
            }));
        return COM_RESULT.COM_SUCCESS;
    }
}