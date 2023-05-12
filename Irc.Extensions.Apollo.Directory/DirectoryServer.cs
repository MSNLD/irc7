using Irc.Commands;
using Irc.Extensions.Apollo.Commands;
using Irc.Extensions.Apollo.Directory.Commands;
using Irc.Extensions.Apollo.Factories;
using Irc.Extensions.Commands;
using Irc.Extensions.Security;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Security;
using Irc7d;
using Version = Irc.Commands.Version;

namespace Irc.Extensions.Apollo.Directory
{
    public class DirectoryServer: Apollo.Objects.Server.ApolloServer
    {
        public string ChatServerIP;
        public string ChatServerPORT;

        public DirectoryServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null, ICredentialProvider ntlmCredentialProvider = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ApolloUserFactory())
        {
            DisableGuestMode = true;
            FlushCommands();
            AddCommand(new Ircvers());
            AddCommand(new Auth());
            AddCommand(new AuthX());
            AddCommand(new Pass());
            AddCommand(new Nick());
            AddCommand(new UserCommand(), Enumerations.EnumProtocolType.IRC, "User");
            AddCommand(new Finds());
            AddCommand(new Prop());
            AddCommand(new Create());
            AddCommand(new Ping());
            AddCommand(new Pong());
            AddCommand(new Version());
        }
    }
}