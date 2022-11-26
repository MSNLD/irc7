using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc4 : Irc3
{
    public Irc4() : base()
    {

    }
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC4;
    }
}