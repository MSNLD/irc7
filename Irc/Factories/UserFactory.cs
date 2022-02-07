using Irc.Enumerations;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Factories;

public interface IUserFactory
{
    public IUser Create(IServer server, IConnection connection);
}

internal class UserUserFactory : IUserFactory
{
    public IUser Create(IServer server, IConnection connection)
    {
        return new User(connection, server.GetProtocol(EnumProtocolType.IRC),
            new DataRegulator(server.MaxInputBytes, server.MaxOutputBytes),
            new FloodProtectionProfile(), new DataStore(connection.GetId().ToString(), "store"), new UserModes(),
            server);
    }
}