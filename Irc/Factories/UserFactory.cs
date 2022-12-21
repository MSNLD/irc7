using Irc.Interfaces;
using Irc.IO;
using Irc.Models.Enumerations;
using Irc.Objects.User;

namespace Irc.Factories;

public interface IUserFactory
{
    public IUser Create(IServer server, IConnection connection);
}

public class UserFactory : IUserFactory
{
    public IUser Create(IServer server, IConnection connection)
    {
        return new User(connection, server.GetProtocol(EnumProtocolType.IRC),
            new DataRegulator(server.MaxInputBytes, server.MaxOutputBytes),
            new FloodProtectionProfile(), new DataStore(connection.GetId().ToString(), "store"), new UserModes(),
            server);
    }
}