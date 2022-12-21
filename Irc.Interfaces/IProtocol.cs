using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IProtocol
{
    ICommand GetCommand(string name);
    Dictionary<string, ICommand> GetCommands();
    void AddCommand(ICommand command, string name = null);
    void FlushCommands();
    EnumProtocolType GetProtocolType();
    string FormattedUser(IChannelMember user);
    string GetFormat(IUser user);
}