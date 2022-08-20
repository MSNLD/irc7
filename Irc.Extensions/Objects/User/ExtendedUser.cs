using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Objects.User;

public class ExtendedUser : global::Irc.Objects.User, IExtendedChatObject
{
    private UserPropCollection _properties;
    public IPropCollection PropCollection => _properties;
    public ExtendedUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, IServer server) :
        base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
    {
        _properties = new UserPropCollection(dataStore);
    }
}