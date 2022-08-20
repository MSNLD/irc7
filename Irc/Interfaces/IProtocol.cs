using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc;

public interface IProtocol
{
    ICommand GetCommand(string name);
    Dictionary<string, ICommand> GetCommands();
    void AddCommand(ICommand command, string name = null);
    void FlushCommands();
    EnumProtocolType GetProtocolType();
    string FormattedUser(IUser user);
    string GetFormat(IUser user);
}