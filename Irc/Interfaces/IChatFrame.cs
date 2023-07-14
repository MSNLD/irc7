using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Interfaces;

public interface IChatFrame
{
    Message Message { get; }
    IServer Server { get; }
    IUser User { get; }
}