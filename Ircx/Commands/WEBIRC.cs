using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    // For webchat proxies
    public class WEBIRC : Command
    {
        //WEBIRC <passwd> ircgw localhost 127.0.0.1 :6s
        public WEBIRC(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0; // to suppress any warnings
            base.DataType = CommandDataType.None;
        }
        
        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data != null) { 
                if (Frame.Message.Data.Count >= 4)
                {
                    String8 Password = Frame.Message.Data[0];
                    String8 Username = Frame.Message.Data[1];
                    String8 Hostname = Frame.Message.Data[2];
                    String8 IP = Frame.Message.Data[3];

                    if ((Username == Program.Config.WebIRCUsername) && (Password == Program.Config.WebIRCPassword)) {
                        Frame.User.Address.Hostname = Hostname;
                        Frame.User.Address.RemoteIP = IP;
                    }
                }
                if (Frame.Message.Data.Count == 5)
                {
                    if (Frame.Message.Data[4].chars.Contains('s'))
                    {
                        //Set secure mode
                        Frame.User.Modes.Secure.Value = 1;
                    }
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
