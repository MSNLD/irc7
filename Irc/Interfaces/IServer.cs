using Irc.Enumerations;
using Irc.Extensions.Security;
using Irc.Interfaces;
using Irc.IO;
using Irc.Security;
using Irc7d;

namespace Irc.Objects.Server;

public interface IServer
{
    DateTime CreationDate { get; }
    bool AnnonymousAllowed { get; }
    int ChannelCount { get; }
    IList<ChatObject> IgnoredUsers { get; }
    IList<string> Info { get; }
    int MaxMessageLength { get; }
    int MaxInputBytes { get; }
    int MaxOutputBytes { get; }
    int NetInvisibleCount { get; }
    int NetServerCount { get; }
    int NetUserCount { get; }
    string SecurityPackages { get; }
    int SysopCount { get; }
    int UnknownConnectionCount { get; }
    string RemoteIP { set; get; }
    Guid Id { get; }
    string ShortId { get; }
    string Name { get; set; }
    void AddUser(IUser user);
    void RemoveUser(IUser user);
    void AddChannel(IChannel channel);
    void RemoveChannel(IChannel channel);
    IChannel CreateChannel(string name);
    IUser CreateUser(IConnection connection);
    IList<IUser> GetUsers();
    IList<IChannel> GetChannels();
    string GetSupportedChannelModes();
    string GetSupportedUserModes();
    IDictionary<EnumProtocolType, IProtocol> GetProtocols();
    Version GetVersion();
    IDataStore GetDataStore();
    IChannel GetChannelByName(string name);
    IProtocol GetProtocol(EnumProtocolType protocolType);
    ISecurityManager GetSecurityManager();
    ICredentialProvider GetCredentialManager();
    void Shutdown();
    string ToString();
}