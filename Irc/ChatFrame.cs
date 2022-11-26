using Irc.Objects;
using Irc.Objects.Server;

namespace Irc;

public class ChatFrame
{
    public readonly Message Message;
    public readonly IServer Server;
    public readonly IUser User;

    public ChatFrame(IServer server, IUser user, Message message)
    {
        Server = server;
        User = user;
        Message = message;
    }
}