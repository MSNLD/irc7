using Irc.Commands;
using Irc.Enumerations;

namespace Irc;

public class Irc : Protocol, IProtocol
{
    public Irc()
    {
        AddCommand(new Privmsg());
        AddCommand(new Ping());
        AddCommand(new Nick());
        AddCommand(new UserCommand(), "User");
        AddCommand(new Mode());
        AddCommand(new Join());
        AddCommand(new Part());
        AddCommand(new Userhost());
        AddCommand(new Commands.Version());
        AddCommand(new Info());
    }

    public new ICommand GetCommand(string name)
    {
        if (Commands.TryGetValue(name, out var command)) return command;

        return null;
    }

    public new EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC;
    }
}