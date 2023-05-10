using Irc.Extensions.Apollo.Objects.User;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo
{
    public static class ApolloRaws
    {
        public static string RPL_JOIN_MSN(IChannelMember member, ApolloUser user, IChannel channel)
        {
            var listedMode = member.GetListedMode();
            var listedModeString = !string.IsNullOrWhiteSpace(listedMode) ? $",{listedMode}" : "";
            return $":{user.GetAddress()} JOIN {member.GetUser().GetProtocol().GetFormat(user)}{listedModeString} :{channel}";
        }
    }
}
