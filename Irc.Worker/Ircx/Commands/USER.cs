using System.Text;
using Irc.Extensions.Security.Packages;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class USER : Command
{
    public USER(CommandCode Code) : base(Code)
    {
        MinParamCount = 4;
        DataType = CommandDataType.None;
    }

    public new COM_RESULT Execute(Frame Frame)
    {
        if (Frame.Message.Data.Count == 4)
        {
            var u = Frame.User;

            var bSetUser = false;
            if (u.Auth != null)
            {
                if (u.Auth.Signature == ANON.SIGNATURE) bSetUser = true;
            }
            else
            {
                bSetUser = true;
            }

            if (bSetUser)
            {
                var iUsernameLen = Frame.Message.Data[0].Length > Program.Config.MaxUsername
                    ? Program.Config.MaxUsername
                    : Frame.Message.Data[0].Length;
                var Userhost = new StringBuilder(iUsernameLen + 1);
                Userhost.Append("~");
                Userhost.Append(Frame.Message.Data[0].Substring(iUsernameLen));
                u.Address.Userhost = Userhost.ToString();
            }

            var Realname = Frame.Message.Data[3];
            if (Realname.Length > Program.Config.MaxRealname)
                Realname = new string(Realname.Substring(Program.Config.MaxRealname));


            if (Realname.Length == 0) Realname = Resources.Wildcard;
            u.Address.RealName = Realname;
        }

        return COM_RESULT.COM_SUCCESS;
    }
}