using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IUser: IChatObject
{
    IServer Server { get; }
    event EventHandler<string> OnSend;
    void BroadcastToChannels(string data, bool excludeUser);
    void AddChannel(IChannel channel, IChannelMember member);
    void RemoveChannel(IChannel channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelMemberInfo(IChannel channel);
    KeyValuePair<IChannel, IChannelMember> GetChannelInfo(string name);
    IDictionary<IChannel, IChannelMember> GetChannels();
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
    IAddress GetAddress();
    bool IsGuest();
    bool IsRegistered();
    bool IsAuthenticated();
    bool IsAnon();
    bool IsSysop();
    bool IsAdministrator();
    bool IsOn(IChannel channel);
    void PromoteToAdministrator();
    void PromoteToSysop();
    void PromoteToGuide();
    bool DisconnectIfOutgoingThresholdExceeded();
    bool DisconnectIfIncomingThresholdExceeded();
    void Register();
    void Authenticate();
    IDataStore GetDataStore();
}