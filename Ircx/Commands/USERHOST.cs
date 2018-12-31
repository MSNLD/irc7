using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class USERHOST: Command
    {
        //IRCX_RPL_USERHOST_302
        public USERHOST(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.User.Registered)
            {
                String8 Userhost = new String8(Frame.User.Name.Length + 1 + Frame.User.Address._address[2].Length);
                Userhost.append(Frame.User.Name);
                Userhost.append((byte)'=');
                Userhost.append(Frame.User.Address._address[2]);
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_USERHOST_302, Data: new String8[] { Userhost }) );
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
