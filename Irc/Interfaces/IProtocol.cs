using Irc.Commands;
using Irc.Enumerations;

namespace Irc;

public interface IProtocol
{
    ICommand GetCommand(string name);
    void AddCommand(ICommand command, string name = null);
    EnumProtocolType GetProtocolType();
}