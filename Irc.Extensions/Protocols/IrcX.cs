using Irc.Extensions.Commands;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Protocols;

public class IrcX : Irc.Protocols.Irc
{
    public IrcX()
    {
        AddCommand(new Commands.Access());
        AddCommand(new Away());
        AddCommand(new Create());
        AddCommand(new Data());
        AddCommand(new Event());
        AddCommand(new Isircx());
        AddCommand(new Kill());
        AddCommand(new Listx());
        AddCommand(new Reply());
        AddCommand(new Request());
        AddCommand(new Whisper());
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRCX;
    }
}