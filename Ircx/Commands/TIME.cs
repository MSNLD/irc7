using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class TIME : Command
    {
        public TIME(CommandCode Code) : base(Code)
        {
            base.RegistrationRequired = true;
            base.MinParamCount = 0;
            base.DataType = CommandDataType.Data;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            //<- :Default-Chat-Community 391 Sky Default-Chat-Community :Saturday, August 24, 2013 17:45:02 GMT
            // dddd, MMMM dd, yyyy HH:mm:ss Z
            Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_RPL_TIME_391, Data: new string[] { new string(DateTime.Now.ToString("dddd, MMMM dd, yyyy HH:mm:ss ")), Frame.Server.TimeZone }));
            return COM_RESULT.COM_SUCCESS;
        } 
    }
}
