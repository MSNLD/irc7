using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Factories;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Apollo.Factories;

internal class ApolloUserFactory : IUserFactory
{
    public IUser Create(IServer server, IConnection connection)
    {
        return new ApolloUser(connection, server.GetProtocol(EnumProtocolType.IRC),
            new DataRegulator(server.MaxInputBytes, server.MaxOutputBytes),
            new FloodProtectionProfile(), new DataStore(connection.GetId().ToString(), "store"), new UserModes(),
            server);
    }
}