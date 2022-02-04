using Irc.Enumerations;
using Irc.Worker.Ircx.Objects;
using User = Irc.Objects.User;

namespace Irc.Interfaces;

public interface IChannel
{
    string GetName();
    IChannelMember GetMember(User User);
    void Send(string message, User u, bool ExcludeSender);
    void Send(string message, User u);
    IChannel Join(User user);
    IChannel Part(User user);
    void SendMessage(User user, string message);
    IList<IChannelMember> GetMembers();
    IChannel SendTopic(User user);
    IChannel SendNames(User user);
    bool Allows(User user);
}