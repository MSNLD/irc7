using Irc.Interfaces;

namespace Irc;

public class ChatFrame : IChatFrame
{
    public IMessage Message { get; }
    public IServer Server { get; }
    public IUser User { get; }

    public ChatFrame(IServer server, IUser user, IMessage message)
    {
        Server = server;
        User = user;
        Message = message;
    }
}