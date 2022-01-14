using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class ADMIN : Command
{
    public ADMIN(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters == null)
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINME_256));
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINLOC1_257,
                Data: new[] {Program.Config.AdminLoc1}));
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINLOC1_258,
                Data: new[] {Program.Config.AdminLoc2}));
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINEMAIL_259,
                Data: new[] {Program.Config.AdminEmail}));
        }
        else
        {
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_OPTIONUNSUPPORTED_555,
                Data: new[] {Frame.Message.Parameters[0]}));
        }

        return true;
    }
}