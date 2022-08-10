using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Commands;
using Irc.Extensions.Apollo.Factories;
using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Apollo.Protocols;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Commands;
using Irc.Extensions.Objects;
using Irc.Extensions.Objects.Server;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc7d;

namespace Irc.Extensions.Apollo.Objects.Server;

public class ApolloServer : ExtendedServer
{
    public ApolloServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ApolloUserFactory())
    {
        securityManager.AddSupportPackage(new GateKeeper());
        securityManager.AddSupportPackage(new GateKeeperPassport(null));
        _protocols.Add(EnumProtocolType.IRC3, new Irc3());
        _protocols.Add(EnumProtocolType.IRC4, new Irc4());
        _protocols.Add(EnumProtocolType.IRC5, new Irc5());
        _protocols.Add(EnumProtocolType.IRC6, new Irc6());
        _protocols.Add(EnumProtocolType.IRC7, new Irc7());
        _protocols.Add(EnumProtocolType.IRC8, new Irc8());

        // Override by adding command support at base IRC
        _protocols[EnumProtocolType.IRC].AddCommand(new Finds());
        _protocols[EnumProtocolType.IRCX].AddCommand(new Finds());
        _protocols[EnumProtocolType.IRC].AddCommand(new Ircvers());
        _protocols[EnumProtocolType.IRCX].AddCommand(new Ircvers());
        _protocols[EnumProtocolType.IRC5].AddCommand(new Prop());

        var modes = new ApolloChannelModes().GetSupportedModes() +
                    new ExtendedMemberModes().GetSupportedModes();
        modes = new string(modes.OrderBy(c => c).ToArray());

        _dataStore.Set("supported.channel.modes", modes);
        _dataStore.Set("supported.user.modes", new ApolloUserModes().GetSupportedModes());
    }

    public new IChannel CreateChannel(string name)
    {
        return new global::Irc.Objects.Channel.Channel(name, new ApolloChannelModes(), new DataStore(name, "store"));
    }
}