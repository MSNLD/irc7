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

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.Message.Data == null)
        {
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371_VERS,
                Data: new[] {Frame.Server.Name}, IData: new[] {Program.Config.major, Program.Config.minor}));
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371,
                Data: new[] {Frame.Server.CreationDate}));
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_ENDOFINFO_374));
        }
        else
        {
            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_OPTIONUNSUPPORTED_555,
                Data: new[] {Frame.Message.Data[0]}));
        }

        return COM_RESULT.COM_SUCCESS;
    }
}