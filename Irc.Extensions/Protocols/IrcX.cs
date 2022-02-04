using Irc.Commands;

namespace Irc.Extensions.Protocols;

public class IrcX : Irc
{
    public IrcX(): base()
    {
        AddCommand(new Auth());
        AddCommand(new Ircx());
    }
}