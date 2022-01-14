using System.Text;
using Irc.Constants;
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

    public new bool Execute(Frame Frame)
    {
        if (Frame.Message.Parameters.Count == 4)
        {
            var u = Frame.User;
            var iUsernameLen = Frame.Message.Parameters[0].Length > Program.Config.MaxUsername
                ? Program.Config.MaxUsername
                : Frame.Message.Parameters[0].Length;
            var Userhost = new StringBuilder(iUsernameLen + 1);
            Userhost.Append("~");
            Userhost.Append(Frame.Message.Parameters[0].Substring(iUsernameLen));
            u.Address.User = Userhost.ToString();

            var Realname = Frame.Message.Parameters[3];
            if (Realname.Length > Program.Config.MaxRealname)
                Realname = new string(Realname.Substring(Program.Config.MaxRealname));


            if (Realname.Length == 0) Realname = Resources.Wildcard;
            u.RealName = Realname;
        }

        return true;
    }
}