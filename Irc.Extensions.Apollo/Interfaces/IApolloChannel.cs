using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.IO;
using Irc.Models.Enumerations;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Apollo.Interfaces;

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
    void Send(string message, IChatObject u = null);
    string GetName();
    IChannelMember GetMember(IUser user);
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
    bool CanBeModifiedBy(IChatObject source);

    EnumIrcError CanModifyMember(IChannelMember source, IChannelMember target,
        EnumChannelAccessLevel requiredLevel);

    void ProcessChannelError(EnumIrcError error, IServer server, IUser source, IChatObject target = null,
        string data = null);

    EnumChannelAccessResult GetAccess(IUser user, string key, bool isGoto = false);
    void SetName(string name);
    bool IsOnChannel(IUser user);
}