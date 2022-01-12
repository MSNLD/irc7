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

    public new COM_RESULT Execute(Frame Frame)
    {
        return COM_RESULT.COM_SUCCESS;
        //if (Frame.Message.Data.Count == 4)
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
        //        int iUsernameLen = (Frame.Message.Data[0].Length > Program.Config.MaxUsername ? Program.Config.MaxUsername : Frame.Message.Data[0].Length);
        //        string Userhost = new string(iUsernameLen + 1);
        //        Userhost.Append("~");
        //        Userhost.Append(Frame.Message.Data[0].bytes.ToString().Substring(iUsernameLen));
        //        u.Address.Userhost = Userhost;
        //    }

        //    string Realname = Frame.Message.Data[3];
        //    if (Realname.Length > Program.Config.MaxRealname) { Realname = new string(Realname.bytes, 0, Program.Config.MaxRealname); }


        //    if (Realname.Length == 0) { Realname = Resources.Wildcard; }
        //    u.Address.RealName = Realname;

        //    u.ObjectType = ObjType.ServerObject;
        //}
        //return COM_RESULT.COM_SUCCESS;
    }
}