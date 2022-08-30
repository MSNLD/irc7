using Irc.Enumerations;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Interfaces;

public interface IChannel
{
    IDataStore ChannelStore { get; }
    string GetName();
    IChannelMember GetMember(IUser User);
    IChannelMember GetMemberByNickname(string nickname);
    void Send(string message, ChatObject u = null);
    void Send(string message);
    IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE);
    IChannel Part(IUser user);
    IChannel Quit(IUser user);
    IChannel Kick(IUser source, IUser target, string reason);
    void SendMessage(IUser user, string message);
    void SendNotice(IUser user, string message);
    IList<IChannelMember> GetMembers();
    bool CanBeModifiedBy(ChatObject source);
    EnumIrcError CanModifyMember(IChannelMember source, IChannelMember target, EnumChannelAccessLevel requiredLevel);
    void ProcessChannelError(EnumIrcError error, IServer server, IUser source, ChatObject target = null, string data = null);
    IChannel SendTopic(IUser user);
    IChannel SendNames(IUser user);
    bool Allows(IUser user);
    IModeCollection GetModes();
    EnumChannelAccessResult GetAccess(IUser user, string key, bool IsGoto = false);
}