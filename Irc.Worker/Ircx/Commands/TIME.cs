using System;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class TIME : Command
{
    public TIME(CommandCode Code) : base(Code)
    {
        RegistrationRequired = true;
        MinParamCount = 0;
        DataType = CommandDataType.Data;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        //<- :Default-Chat-Community 391 Sky Default-Chat-Community :Saturday, August 24, 2013 17:45:02 GMT
        // dddd, MMMM dd, yyyy HH:mm:ss Z
        Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_TIME_391,
            Data: new[] {new(DateTime.Now.ToString("dddd, MMMM dd, yyyy HH:mm:ss ")), Frame.Server.ServerFields.TimeZone}));
        return COM_RESULT.COM_SUCCESS;
    }
}