using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc5 : Irc4
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC5;
    }

    public override string GetProfileString(ApolloUser apolloUser) => apolloUser.GetProfile().Irc5_ToString();
}