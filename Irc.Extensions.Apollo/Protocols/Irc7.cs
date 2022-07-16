using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc7 : Irc6
{
    public override string FormattedUser(IUser user)
    {
        return $"H,U,GPX,{user.GetAddress().Nickname}";
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC7;
    }
    public override string GetProfileString(ApolloUser apolloUser) => apolloUser.GetProfile().Irc7_ToString();
}