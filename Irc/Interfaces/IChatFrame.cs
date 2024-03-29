using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Interfaces;

public interface IChatFrame
{
    long SequenceId { get; set; }
    Message Message { get; }
    IServer Server { get; }
    IUser User { get; }
}