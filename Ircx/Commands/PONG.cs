using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class PONG : Command
    {

        public PONG(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.DataType = CommandDataType.None;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
