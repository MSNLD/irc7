using Irc.Enumerations;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc8 : Irc7
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC8;
    }
}