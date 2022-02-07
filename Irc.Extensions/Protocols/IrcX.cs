using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Protocols;

public class IrcX : Irc
{
    public IrcX()
    {
        AddCommand(new Auth());
        AddCommand(new Ircx());
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRCX;
    }
}