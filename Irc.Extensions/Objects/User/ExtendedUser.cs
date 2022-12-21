using Irc.Extensions.Access.User;
using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Objects.User;

public class ExtendedUser : global::Irc.Objects.User.User, IExtendedChatObject
{
    private readonly UserAccess _accessList = new();
    private readonly UserPropCollection _properties;

    public ExtendedUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, IServer server) :
        base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
    {
        _properties = new UserPropCollection(dataStore);
    }

    public IPropCollection PropCollection => _properties;

    public IAccessList AccessList => _accessList;
}