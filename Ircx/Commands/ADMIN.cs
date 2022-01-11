using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class ADMIN : Command
    {

        public ADMIN(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = true;
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data == null)
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINME_256));
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINLOC1_257, Data: new string[] { Program.Config.AdminLoc1 }));
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINLOC1_258, Data: new string[] { Program.Config.AdminLoc2 }));
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_ADMINEMAIL_259, Data: new string[] { Program.Config.AdminEmail }));
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_OPTIONUNSUPPORTED_555, Data: new string[] { Frame.Message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
