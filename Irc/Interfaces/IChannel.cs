using Irc.Enumerations;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Interfaces;

public interface IChannel: IChatObject
{
    public IDataStore ChannelStore { get; }
    public string GetName();
    public IChannelMember GetMember(IUser user);
    public IChannelMember GetMemberByNickname(string nickname);
    public void Send(string message, ChatObject u = null);
    public void Send(string message);
    public IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE);
    public IChannel Part(IUser user);
    public IChannel Quit(IUser user);
    public IChannel Kick(IUser source, IUser target, string reason);
    public void SendMessage(IUser user, string message);
    public void SendNotice(IUser user, string message);
    public IList<IChannelMember> GetMembers();
    public bool CanBeModifiedBy(ChatObject source);
    public EnumIrcError CanModifyMember(IChannelMember source, IChannelMember target, EnumChannelAccessLevel requiredLevel);

    public void ProcessChannelError(EnumIrcError error, IServer server, IUser source, ChatObject target = null,
        string data = null);

    public IChannel SendTopic(IUser user);
    public IChannel SendNames(IUser user);
    public bool Allows(IUser user);
    public IModeCollection GetModes();
    public EnumChannelAccessResult GetAccess(IUser user, string key, bool isGoto = false);
}