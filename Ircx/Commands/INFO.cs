using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class INFO : Command
    {

        public INFO(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 0;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            if (Frame.Message.Data == null)
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371_VERS, Data: new string[] { Frame.Server.Name }, IData: new int[] { Program.Config.major, Program.Config.minor }));
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_INFO_371, Data: new string[] { Frame.Server.CreationDate }));
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_RPL_ENDOFINFO_374));
            }
            else
            {
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_OPTIONUNSUPPORTED_555, Data: new string[] { Frame.Message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
