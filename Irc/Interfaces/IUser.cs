using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Objects;

public interface IUser
{
    IServer Server { get; }
    Guid Id { get; }
    string ShortId { get; }
    string Name { get; set; }
    event EventHandler<string> OnSend;
    void BroadcastToChannels(string data, bool ExcludeUser);
    void AddChannel(IChannel channel, IChannelMember member);
    void RemoveChannel(IChannel channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelMemberInfo(IChannel channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelInfo(string Name);
    void Send(string message);
    void Flush();
    void Disconnect(string message);
    IDataRegulator GetDataRegulator();
    IFloodProtectionProfile GetFloodProtectionProfile();
    ISupportPackage GetSupportPackage();
    void SetSupportPackage(ISupportPackage supportPackage);
    void SetProtocol(IProtocol protocol);
    IProtocol GetProtocol();
    IConnection GetConnection();
    EnumUserAccessLevel GetLevel();
    Address GetAddress();
    bool IsGuest();
    bool IsRegistered();
    bool IsAuthenticated();
    bool IsAnon();
    bool DisconnectIfOutgoingThresholdExceeded();
    bool DisconnectIfIncomingThresholdExceeded();
    string ToString();
    void Register();
    void Authenticate();
    IDataStore GetDataStore();
}