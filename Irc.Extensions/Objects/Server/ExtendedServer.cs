using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Commands;
using Irc.Extensions.Factories;
using Irc.Extensions.Objects.Channel;
using Irc.Extensions.Objects.User;
using Irc.Extensions.Protocols;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Credentials;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Objects.Server;

public class ExtendedServer : global::Irc.Objects.Server.Server, IServer
{
    public ExtendedServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ExtendedUserFactory())
    {
        securityManager.AddSupportPackage(new Security.Packages.NTLM(new NtlmProvider()));

        _protocols.Add(EnumProtocolType.IRCX, new IrcX());
        AddCommand(new Auth());
        AddCommand(new Ircx());
        AddCommand(new Prop());

        var modes = new ExtendedChannelModes().GetSupportedModes();
        modes = new string(modes.OrderBy(c => c).ToArray());
        _dataStore.Set("supported.channel.modes", modes);
        _dataStore.Set("supported.user.modes", new ExtendedUserModes().GetSupportedModes());
    }

    public new IChannel CreateChannel(string name)
    {
        return new global::Irc.Extensions.Objects.Channel.ExtendedChannel(name, new ExtendedChannelModes(), new DataStore(name, "store"));
    }
}