using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc;

public interface IProtocol
{
    ICommand GetCommand(string name);
    void AddCommand(ICommand command, string name = null);
    void FlushCommands();
    EnumProtocolType GetProtocolType();
    string FormattedUser(IUser user);
}