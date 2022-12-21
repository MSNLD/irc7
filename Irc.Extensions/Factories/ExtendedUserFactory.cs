using Irc.Extensions.Objects.User;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Models.Enumerations;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Factories;

public class ExtendedUserFactory : IUserFactory
{
    public IUser Create(IServer server, IConnection connection)
    {
        return new ExtendedUser(connection, server.GetProtocol(EnumProtocolType.IRC),
            new DataRegulator(server.MaxInputBytes, server.MaxOutputBytes),
            new FloodProtectionProfile(), new DataStore(connection.GetId().ToString(), "store"),
            new ExtendedUserModes(),
            server);
    }
}