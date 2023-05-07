using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Commands;
using Irc.Extensions.Factories;
using Irc.Extensions.Interfaces;
using Irc.Extensions.Objects.Channel;
using Irc.Extensions.Objects.User;
using Irc.Extensions.Protocols;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Credentials;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc.Security;
using Irc7d;

namespace Irc.Extensions.Objects.Server;

public class ExtendedServer : global::Irc.Objects.Server.Server, IServer, IExtendedServerObject
{
    public ExtendedServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ExtendedUserFactory())
    {
        AddProtocol(EnumProtocolType.IRCX, new IrcX());
        AddCommand(new Auth());
        AddCommand(new AuthX());
        AddCommand(new Ircx());
        AddCommand(new Prop());

        var modes = new ExtendedChannelModes().GetSupportedModes();
        modes = new string(modes.OrderBy(c => c).ToArray());
        _dataStore.Set("supported.channel.modes", modes);
        _dataStore.Set("supported.user.modes", new ExtendedUserModes().GetSupportedModes());
    }

    public override IChannel CreateChannel(string name)
    {
        return new ExtendedChannel(name, new ExtendedChannelModes(), new DataStore(name, "store"));
    }

    public virtual void ProcessCookie(IUser user, string name, string value)
    {
        // IRCX Does not use this
    }

    // Ircx
    protected EnumChannelAccessResult CheckAuthOnly()
    {
        if (Modes.GetModeChar(ExtendedResources.ChannelModeAuthOnly) == 1) return EnumChannelAccessResult.ERR_AUTHONLYCHAN;
        return EnumChannelAccessResult.NONE;
    }

    protected EnumChannelAccessResult CheckSecureOnly()
    {
        // TODO: Whatever this is...
        return EnumChannelAccessResult.ERR_SECUREONLYCHAN;
    }
}