using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class ISIRCX : Command
    {
        public ISIRCX(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = false;
            base.DataType = CommandDataType.Standard;
            base.ForceFloodCheck = true;
        }
        public new COM_RESULT Execute(Frame Frame)
        {
            IRCX.ProcessIRCXReply(Frame);
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
