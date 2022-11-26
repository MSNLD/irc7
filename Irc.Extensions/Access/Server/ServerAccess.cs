using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Access.Server
{
    public class ServerAccess: AccessList
    {
        public ServerAccess()
        {
            accessEntries = new Dictionary<Enumerations.EnumAccessLevel, List<AccessEntry>>()
            {
                { Enumerations.EnumAccessLevel.OWNER, new List<AccessEntry>() },
                { Enumerations.EnumAccessLevel.HOST, new List<AccessEntry>() },
                { Enumerations.EnumAccessLevel.VOICE, new List<AccessEntry>() },
                { Enumerations.EnumAccessLevel.DENY, new List<AccessEntry>() },
            };
        }
    }
}
