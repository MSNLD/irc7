using Irc.Objects;
using Irc.Objects.Server;

namespace Irc;

public class ChatFrame
{
    public readonly Message Message;
    public readonly Server Server;
    public readonly User User;

    public ChatFrame(Server server, User user, Message message)
    {
        Server = server;
        User = user;
        Message = message;
    }
}