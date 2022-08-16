using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Protocols;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

public class Irc3 : IrcX
{
    public Irc3() : base()
    {
        //AddCommand(new Ircvers());
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC3;
    }

    public override string FormattedUser(IUser user)
    {
        return user.GetAddress().Nickname;
    }
}