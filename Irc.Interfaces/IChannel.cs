using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IChannel: IChatObject
{
    public IDataStore ChannelStore { get; }
    public string GetName();
    public IChannelMember GetMember(IUser user);
    public IChannelMember GetMemberByNickname(string nickname);
    public IChannel Join(IUser user, EnumChannelAccessResult accessResult = EnumChannelAccessResult.NONE);
    public IChannel Part(IUser user);
    public IChannel Quit(IUser user);
    public IChannel Kick(IUser source, IUser target, string reason);
    public void SendMessage(IUser user, string message);
    public void SendNotice(IUser user, string message);
    public IList<IChannelMember> GetMembers();
    public bool CanBeModifiedBy(IChatObject source);
    public EnumIrcError CanModifyMember(IChannelMember source, IChannelMember target, EnumChannelAccessLevel requiredLevel);

    public void ProcessChannelError(EnumIrcError error, IServer server, IUser source, IChatObject target = null,
        string data = null);

    public IChannel SendTopic(IUser user);
    public IChannel SendNames(IUser user);
    public bool Allows(IUser user);
    public EnumChannelAccessResult GetAccess(IUser user, string key, bool isGoto = false);
}