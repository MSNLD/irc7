using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc7d;

namespace Irc.Extensions.Objects.User
{
    public class ExtendedUser: global::Irc.Objects.User
    {
        public ExtendedUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator, IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, global::Irc.Objects.Server.Server server) : base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
        {
        }
    }
}
