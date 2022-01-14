using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class VERSION : Command
{
    public VERSION(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_VERSION_351,
            IData: new[]
            {
                Program.Config.major, Program.Config.minor, Program.Config.build, Program.Config.major,
                Program.Config.minor
            }));
        return true;
    }
}