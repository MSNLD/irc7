using Irc.Enumerations;
using Irc.Extensions.Protocols;
using Irc.Interfaces;

namespace Irc.Extensions.Apollo.Protocols;

public class Irc3 : IrcX
{
    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC3;
    }

    public override string FormattedUser(IChannelMember member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';
        return $"{modeChar}{member.GetUser().GetAddress().Nickname}";
    }
}