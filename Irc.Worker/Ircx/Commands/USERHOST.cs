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

    public new bool Execute(Frame Frame)
    {
        if (Frame.User.Registered)
        {
            var userHost = $"{Frame.User.Address.Nickname}!~{Frame.User.Address.GetUserHost()}";
            var userHostReply = new StringBuilder(Frame.User.Name.Length + 1 + userHost.Length);
            userHostReply.Append(Frame.User.Name);
            userHostReply.Append('=');
            userHostReply.Append(userHost);
            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERHOST_302,
                Data: new[] {userHostReply.ToString()}));
        }

        return true;
    }
}