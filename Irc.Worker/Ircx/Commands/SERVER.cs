using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

public class SERVER : Command
{
    // This was the beginning of trying to implement multi-server support
    public SERVER(CommandCode Code) : base(Code)
    {
        MinParamCount = 4;
        DataType = CommandDataType.None;
        PreRegistration = true;
        RegistrationRequired = false;
    }

    public new bool Execute(Frame Frame)
    {
        return true;
        //if (Frame.Message.Parameters.Count == 4)
        //{
        //    User u = Frame.User;

        //    bool bSetUser = false;
        //    if (u.Auth != null)
        //    {
        //        if (u.Auth.Signature == Authentication.ANON.SIGNATURE)
        //        {
        //            bSetUser = true;
        //        }
        //    }
        //    else
        //    {
        //        bSetUser = true;
        //    }

        //    if (bSetUser == true)
        //    {
        //        int iUsernameLen = (Frame.Message.Parameters[0].Length > Program.Config.MaxUsername ? Program.Config.MaxUsername : Frame.Message.Parameters[0].Length);
        //        string User = new string(iUsernameLen + 1);
        //        User.Append("~");
        //        User.Append(Frame.Message.Parameters[0].bytes.ToString().Substring(iUsernameLen));
        //        u.Address.User = User;
        //    }

        //    string Realname = Frame.Message.Parameters[3];
        //    if (Realname.Length > Program.Config.MaxRealname) { Realname = new string(Realname.bytes, 0, Program.Config.MaxRealname); }


        //    if (Realname.Length == 0) { Realname = Resources.Wildcard; }
        //    u.Address.RealName = Realname;

        //    u.ObjectType = ObjType.ServerObject;
        //}
        //return true;
    }
}