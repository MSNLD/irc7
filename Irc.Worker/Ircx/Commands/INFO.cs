using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class INFO : Command
{
    public INFO(CommandCode Code) : base(Code)
    {
        MinParamCount = 0;
        RegistrationRequired = true;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters == null)
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371_VERS,
                Data: new[] {Frame.Server.Name}, IData: new[] {Program.Config.major, Program.Config.minor}));
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371,
                Data: new[] {Frame.Server.ServerFields.CreationDate}));
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_ENDOFINFO_374));
        }
        else
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_OPTIONUNSUPPORTED_555,
                Data: new[] {Frame.Message.Parameters[0]}));
        }

        return true;
    }
}