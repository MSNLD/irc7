using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc6 : Irc5
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC6;
    }
}