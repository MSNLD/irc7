using Irc.Extensions.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions
{
    public static class IrcxRaws
    {
        public static string IRCX_RPL_PROPLIST_818(IServer server, IUser user, IExtendedChatObject chatObject, string propName, string propValue)
        {
            return $":{server} 818 {user} {chatObject} {propName} :{propValue}";
        }

        public static string IRCX_RPL_PROPEND_819(IServer server, IUser user, IExtendedChatObject chatObject)
        {
            return $":{server} 819 {user} {chatObject} :End of properties";
        }
    }
}
