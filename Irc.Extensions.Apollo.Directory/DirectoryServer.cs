using Irc.Commands;
using Irc.Extensions.Apollo.Commands;
using Irc.Extensions.Apollo.Factories;
using Irc.Extensions.Security;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Security;
using Irc7d;

namespace Irc.Extensions.Apollo.Directory
{
    public class DirectoryServer: Apollo.Objects.Server.ApolloServer
    {
        public DirectoryServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null, ICredentialProvider ntlmCredentialProvider = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ApolloUserFactory())
        {
            FlushCommands();
            AddCommand(new Auth());
            AddCommand(new Pass());
            AddCommand(new Nick());
            AddCommand(new UserCommand(), Enumerations.EnumProtocolType.IRC, "User");
            AddCommand(new Finds());
        }
    }
}