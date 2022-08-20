using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;

namespace Irc;

public class Protocol : IProtocol
{
    protected Dictionary<string, ICommand> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public ICommand GetCommand(string name)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, ICommand> GetCommands() => Commands;

    public EnumProtocolType GetProtocolType()
    {
        throw new NotImplementedException();
    }

    public void AddCommand(ICommand command, string name = null)
    {
        if (!Commands.ContainsKey(name == null ? command.GetName() : name)) Commands.Add(name ?? command.GetName(), command);
    }
    public void FlushCommands()
    {
        Commands.Clear();
    }

    public virtual string FormattedUser(IUser user)
    {
        return user.GetAddress().Nickname;
    }

    public virtual string GetFormat(IUser user)
    {
        throw new NotImplementedException();
    }
}