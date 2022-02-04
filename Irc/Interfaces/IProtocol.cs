using Irc.Commands;
using Irc.Worker.Ircx.Objects;

namespace Irc;

public interface IProtocol
{
    ICommand GetCommand(string name);
    EnumProtocolType GetProtocolType();
}