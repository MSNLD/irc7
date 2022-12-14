using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Apollo.Objects.Channel;

public interface IApolloChannel
{
    IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE);
    string ToString();
    EnumUserAccessLevel Level { get; }
    IModeCollection Modes { get; }
    Guid Id { get; }
    string ShortId { get; }
    string Name { get; set; }
    IDataStore ChannelStore { get; }
    IPropCollection PropCollection { get; }
    IAccessList AccessList { get; }
    IModeCollection GetModes();
    void Send(string message);
    void Send(string message, ChatObject u = null);
    string GetName();
    IChannelMember GetMember(IUser User);
    IChannelMember GetMemberByNickname(string nickname);
    bool Allows(IUser user);
    IChannel SendTopic(IUser user);
    IChannel SendNames(IUser user);
    IChannel Part(IUser user);
    IChannel Quit(IUser user);
    IChannel Kick(IUser source, IUser target, string reason);
    void SendMessage(IUser user, string message);
    void SendNotice(IUser user, string message);
    IList<IChannelMember> GetMembers();
    bool CanBeModifiedBy(ChatObject source);

    EnumIrcError CanModifyMember(IChannelMember source, IChannelMember target,
        EnumChannelAccessLevel requiredLevel);

    void ProcessChannelError(EnumIrcError error, IServer server, IUser source, ChatObject target = null,
        string data = null);

    EnumChannelAccessResult GetAccess(IUser user, string key, bool IsGoto = false);
    void SetName(string Name);
    bool IsOnChannel(IUser user);
}