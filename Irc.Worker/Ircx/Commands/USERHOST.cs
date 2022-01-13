using System.Text;
using Irc.Constants;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class USERHOST : Command
{
    //IRCX_RPL_USERHOST_302
    public USERHOST(CommandCode Code) : base(Code)
    {
        MinParamCount = 0;
        RegistrationRequired = true;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.User.Registered)
        {
            var Userhost = new StringBuilder(Frame.User.Name.Length + 1 + Frame.User.Address._address[2].Length);
            Userhost.Append(Frame.User.Name);
            Userhost.Append('=');
            Userhost.Append(Frame.User.Address._address[2]);
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERHOST_302,
                Data: new[] {Userhost.ToString()}));
        }

        return COM_RESULT.COM_SUCCESS;
    }
}