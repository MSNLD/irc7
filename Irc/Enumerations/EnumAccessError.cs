using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Enumerations
{
    public enum EnumAccessError
    {
        IRCERR_BADLEVEL,
        IRCERR_DUPACCESS,
        IRCERR_MISACCESS,
        IRCERR_TOOMANYACCESSES,
        IRCERR_TOOMANYARGUMENTS,
        IRCERR_BADCOMMAND,
        IRCERR_NOTSUPPORTED,
        IRCERR_NOACCESS,
        IRCERR_INCOMPLETE,
        SUCCESS
    }
}
