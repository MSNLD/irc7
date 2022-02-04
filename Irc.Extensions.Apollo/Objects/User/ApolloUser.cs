using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Objects.User;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Apollo.Objects.User
{
    public class ApolloUser: ExtendedUser
    {
        public ApolloUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator, IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, Server server) : base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
        {
        }
    }
}
