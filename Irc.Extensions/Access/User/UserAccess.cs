using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Access.User
{
    public class UserAccess: AccessList
    {
        public UserAccess()
        {
            accessEntries = new Dictionary<Enumerations.EnumAccessLevel, List<AccessEntry>>()
            {
                { Enumerations.EnumAccessLevel.VOICE, new List<AccessEntry>() },
                { Enumerations.EnumAccessLevel.DENY, new List<AccessEntry>() },
            };
        }
    }
}
