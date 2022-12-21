namespace Irc.Interfaces;

public interface IChatFrame
{
    IMessage Message { get; }
    IServer Server { get; }
    IUser User { get; }
}