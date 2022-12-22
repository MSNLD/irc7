﻿using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface IServer
{
    DateTime CreationDate { get; }
    bool AnnonymousAllowed { get; }
    int ChannelCount { get; }
    IList<IChatObject> IgnoredUsers { get; }
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
    string RemoteIp { set; get; }
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
    IUser GetUserByNickname(string nickname);
    IList<IUser> GetUsersByList(string nicknames, char separator);
    IList<IUser> GetUsersByList(List<string> nicknames, char separator);
    IList<IChannel> GetChannels();
    string GetSupportedChannelModes();
    string GetSupportedUserModes();
    IDictionary<EnumProtocolType, IProtocol> GetProtocols();
    Version GetVersion();
    IDataStore GetDataStore();
    IChannel GetChannelByName(string name);
    IChatObject GetChatObject(string name);
    IProtocol GetProtocol(EnumProtocolType protocolType);
    ISecurityManager GetSecurityManager();
    ICredentialProvider GetCredentialManager();
    void Shutdown();
    string ToString();
    string[] GetMotd();
    void SetMotd(string motd);
}