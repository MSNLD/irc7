using Irc.Extensions.Objects.User;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Apollo.Objects.User;

public class ApolloUser : ExtendedUser
{
    ApolloProfile Profile { get; set; } = new ApolloProfile();

    public ApolloUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, IServer server) :
        base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
    {
    }

    public ApolloProfile GetProfile() => Profile;
}