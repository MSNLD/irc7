using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc;

public class ChatFrame: IChatFrame
{
    public Message Message { set; get; }
    public IServer Server { set; get; }
    public IUser User { set; get; }
}