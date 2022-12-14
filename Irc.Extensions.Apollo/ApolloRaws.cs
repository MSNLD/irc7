using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Interfaces;

namespace Irc.Extensions.Apollo;

public static class ApolloRaws
{
    public static string RPL_JOIN_MSN(IProtocol protocol, ApolloUser user, IApolloChannel channel)
    {
        return $":{user.GetAddress()} JOIN {protocol.GetFormat(user)} :{channel}";
    }
}