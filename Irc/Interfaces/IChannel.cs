using Irc.IO;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IChannel
{
    IDataStore ChannelStore { get; }
    string GetName();
    IChannelMember GetMember(IUser User);
    void Send(string message, IUser u, bool ExcludeSender);
    void Send(string message, IUser u);
    IChannel Join(IUser user);
    IChannel Part(IUser user);
    IChannel Quit(IUser user);
    void SendMessage(IUser user, string message);
    void SendNotice(IUser user, string message);
    IList<IChannelMember> GetMembers();
    IChannel SendTopic(IUser user);
    IChannel SendNames(IUser user);
    bool Allows(IUser user);
    IModeCollection GetModes();
}