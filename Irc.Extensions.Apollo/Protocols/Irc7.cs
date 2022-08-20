using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc7 : Irc6
{
    public Irc7() : base()
    {

    }
    public override string FormattedUser(IUser user)
    {
        return ((ApolloUser)user).GetProfile().Irc7_ToString() + $",{user}";
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC7;
    }

    public override string GetFormat(IUser user)
    {
        return ((ApolloUser)user).GetProfile().Irc7_ToString();
    }
}