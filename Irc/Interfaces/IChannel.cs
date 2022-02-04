using Irc.IO;
using Irc.Objects;

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
    IModeCollection GetModes();
    IDataStore ChannelStore { get; }
}