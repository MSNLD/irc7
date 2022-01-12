using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class PASS : Command
{
    public PASS(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        PreRegistration = true;
        DataType = CommandDataType.None;
        ForceFloodCheck = true;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.User.Auth == null)
        {
            Frame.User.Auth = new ANON();
            Frame.User.Auth.UserCredentials = new Credentials();
            Frame.User.Auth.UserCredentials.Password = Frame.Message.Data[0];
        }

        return COM_RESULT.COM_SUCCESS;
    }
}