using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class PING : Command
    {

        public PING(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.RPL_PONG));
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
