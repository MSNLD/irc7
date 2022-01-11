using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;
using System.Text;
using Core.CSharpTools;

namespace Core.Ircx.Commands
{
    public class USER : Command
    {
        public USER(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 4;
            base.DataType = CommandDataType.None;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data.Count == 4) { 
                User u = Frame.User;

                bool bSetUser = false;
                if (u.Auth != null)
                {
                    if (u.Auth.Signature == Authentication.ANON.SIGNATURE)
                    {
                        bSetUser = true;
                    }
                }
                else
                {
                    bSetUser = true;
                }

                if (bSetUser == true) { 
                    int iUsernameLen = (Frame.Message.Data[0].Length > Program.Config.MaxUsername ? Program.Config.MaxUsername : Frame.Message.Data[0].Length);
                    StringBuilder Userhost = new StringBuilder(iUsernameLen + 1);
                    Userhost.Append("~");
                    Userhost.Append(Frame.Message.Data[0].ToString().Substring(iUsernameLen));
                    u.Address.Userhost = Userhost.ToString();
                }

                string Realname = Frame.Message.Data[3];
                if (Realname.Length > Program.Config.MaxRealname) { Realname = new string(Realname.ToString().Substring(Program.Config.MaxRealname)); }
                

                if (Realname.Length == 0) { Realname = Resources.Wildcard; }
                u.Address.RealName = Realname;
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
