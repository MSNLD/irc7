using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Interfaces;

public interface IExtendedChannel: IChannel, IExtendedChatObject
{
    void SetName(string name);
    bool IsOnChannel(IUser user);
}