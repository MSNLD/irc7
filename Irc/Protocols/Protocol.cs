using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Protocols;

public class Protocol : IProtocol
{
    protected Dictionary<string, ICommand> Commands = new(StringComparer.InvariantCultureIgnoreCase);

    public ICommand GetCommand(string name)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, ICommand> GetCommands()
    {
        return Commands;
    }

    public EnumProtocolType GetProtocolType()
    {
        throw new NotImplementedException();
    }

    public void AddCommand(ICommand command, string name = null)
    {
        if (!Commands.ContainsKey(name == null ? command.GetName() : name))
            Commands.Add(name ?? command.GetName(), command);
    }

    public void FlushCommands()
    {
        Commands.Clear();
    }

    public virtual string FormattedUser(IChannelMember member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';
        return $"{modeChar}{member.GetUser().GetAddress().Nickname}";
    }

    public virtual string GetFormat(IUser user)
    {
        throw new NotImplementedException();
    }
}