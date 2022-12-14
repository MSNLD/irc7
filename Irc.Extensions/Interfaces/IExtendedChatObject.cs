using Irc.Interfaces;

namespace Irc.Extensions.Interfaces;

public interface IExtendedChatObject : IChatObject
{
    public IPropCollection PropCollection { get; }
    public IAccessList AccessList { get; }
}