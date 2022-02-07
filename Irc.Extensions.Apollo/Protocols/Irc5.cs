using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc5 : Irc4
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC5;
    }
}