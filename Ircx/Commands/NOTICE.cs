using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class NOTICE : Command
    {
        public NOTICE(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 2;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
        }
        
        public new COM_RESULT Execute(Frame Frame)
        {
            return PRIVMSG.ProcessMessage(Frame, false);
        }
    }
}
