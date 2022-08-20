using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc5 : Irc4
{
    public Irc5() : base()
    {

    }
    public override string FormattedUser(IUser user)
    {
        return ((ApolloUser)user).GetProfile().Irc5_ToString() + $",{user}";
    }
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC5;
    }

    public override string GetFormat(IUser user)
    {
        return ((ApolloUser)user).GetProfile().Irc5_ToString();
    }
}