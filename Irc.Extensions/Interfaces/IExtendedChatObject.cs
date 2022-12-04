using Irc.Interfaces;

namespace Irc.Extensions.Interfaces;

public interface IExtendedChatObject : IChatObject
{
    IPropCollection PropCollection { get; }
    IAccessList AccessList { get; }
}