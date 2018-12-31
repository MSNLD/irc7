using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class VERSION : Command
    {

        public VERSION(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = true;
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_VERSION_351, IData: new int[] { Program.Config.major, Program.Config.minor, Program.Config.build, Program.Config.major, Program.Config.minor }));
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
