using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Objects.Channel;
using Irc.Extensions.Objects.User;
using Irc.Extensions.Protocols;
using Irc.Extensions.Security;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc7d;

namespace Irc.Extensions.Objects.Server
{
    public class ExtendedServer: global::Irc.Objects.Server.Server
    {
        public ExtendedServer(ISocketServer socketServer, ISecurityManager securityManager,
            IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
            ICommandCollection commands) : base(socketServer, securityManager, floodProtectionManager, dataStore, channels, commands)
        {
            _protocols[EnumProtocolType.IRC].AddCommand(new Auth());
            _protocols[EnumProtocolType.IRC].AddCommand(new Ircx());
            _protocols.Add(EnumProtocolType.IRCX, new IrcX());

            var modes = (new ExtendedChannelModes()).GetSupportedModes() +
                        (new ExtendedMemberModes()).GetSupportedModes();
            modes = new string(modes.OrderBy(c => c).ToArray());
            _dataStore.Set("supported.channel.modes", modes);
            _dataStore.Set("supported.user.modes", (new ExtendedUserModes()).GetSupportedModes());
        }

        public new IChannel CreateChannel(string name)
        {
            return new global::Irc.Objects.Channel.Channel(name, new ExtendedChannelModes(), new DataStore(name, "store", true));
        }

        public new global::Irc.Objects.User CreateUser(IConnection connection)
        {
            return new global::Irc.Objects.User(connection, _protocols.First().Value, new DataRegulator(MaxInputBytes, MaxOutputBytes),
                new FloodProtectionProfile(), new DataStore(Name, "store"), new ExtendedUserModes(), this);
        }
    }
}
