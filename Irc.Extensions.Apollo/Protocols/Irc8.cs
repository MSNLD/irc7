using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc8 : Irc7
{
    public Irc8() : base()
    {

    }
    public override string FormattedUser(IUser user)
    {
        return ((ApolloUser)user).GetProfile().ToString();
    }
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC8;
    }
}