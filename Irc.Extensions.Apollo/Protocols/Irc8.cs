using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc8 : Irc7
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC8;
    }
    public override string GetProfileString(ApolloUser apolloUser) => apolloUser.GetProfile().ToString();
}