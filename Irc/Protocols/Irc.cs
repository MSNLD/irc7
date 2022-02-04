using Irc.Commands;
using Irc.Worker.Ircx.Objects;

namespace Irc
{
    public class Irc: Protocol, IProtocol
    {
        public Irc()
        {
            Commands.Add(nameof(Ircx), new Ircx());
            Commands.Add(nameof(Privmsg), new Privmsg());
            Commands.Add(nameof(Ping), new Ping());
            Commands.Add(nameof(Nick), new Nick());
            Commands.Add("User", new UserCommand());
            Commands.Add(nameof(Auth), new Auth());
            Commands.Add(nameof(Ircvers), new Ircvers());
            Commands.Add(nameof(Mode), new Mode());
            Commands.Add(nameof(Join), new Join());
            Commands.Add(nameof(Part), new Part());
            Commands.Add(nameof(Userhost), new Userhost());
        }
        public new ICommand GetCommand(string name)
        {
            if (Commands.TryGetValue(name, out var command))
            {
                return command;
            }

            return null;
        }

        public new EnumProtocolType GetProtocolType() => EnumProtocolType.IRC;
    }
}