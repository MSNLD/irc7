using Irc.Commands;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Commands;
using Irc.Extensions.Apollo.Factories;
using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Apollo.Protocols;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Apollo.Security.Passport;
using Irc.Extensions.Commands;
using Irc.Extensions.Objects;
using Irc.Extensions.Objects.Server;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Factories;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Security;
using Irc7d;

namespace Irc.Extensions.Apollo.Objects.Server;

public class ApolloServer : ExtendedServer
{
    PassportV4 passport;
    public ApolloServer(ISocketServer socketServer, ISecurityManager securityManager,
        IFloodProtectionManager floodProtectionManager, IDataStore dataStore, IList<IChannel> channels,
        ICommandCollection commands, IUserFactory userFactory = null) : base(socketServer, securityManager,
        floodProtectionManager, dataStore, channels, commands, userFactory ?? new ApolloUserFactory())
    {
        passport = new PassportV4(dataStore.Get("Passport.V4.AppID"), dataStore.Get("Passport.V4.Secret"));
        securityManager.AddSupportPackage(new GateKeeper());
        securityManager.AddSupportPackage(new GateKeeperPassport(new PassportProvider(passport)));

        AddProtocol(EnumProtocolType.IRC3, new Irc3());
        AddProtocol(EnumProtocolType.IRC4, new Irc4());
        AddProtocol(EnumProtocolType.IRC5, new Irc5());
        AddProtocol(EnumProtocolType.IRC6, new Irc6());
        AddProtocol(EnumProtocolType.IRC7, new Irc7());
        AddProtocol(EnumProtocolType.IRC8, new Irc8());

        // Override by adding command support at base IRC
        AddCommand(new Auth());
        AddCommand(new Ircvers());
        AddCommand(new Prop());

        var modes = new ApolloChannelModes().GetSupportedModes();
        modes = new string(modes.OrderBy(c => c).ToArray());

        _dataStore.Set("supported.channel.modes", modes);
        _dataStore.Set("supported.user.modes", new ApolloUserModes().GetSupportedModes());
    }

    public override IChannel CreateChannel(string name)
    {
        return new ApolloChannel(name, new ApolloChannelModes(), new DataStore(name, "store"));
    }

    public override void ProcessCookie(IUser user, string name, string value)
    {
        if (name == "MSNREGCOOKIE" && user.IsAuthenticated() && !user.IsRegistered())
        {
            var nickname = passport.ValidateRegCookie(value);
            if (nickname != null)
            {
                user.GetDataRegulator().PushIncoming(
                    new Message(user.GetProtocol(), $"NICK {nickname}")
                    );
            }
        }
        else if (name == "SUBSCRIBERINFO" && user.IsAuthenticated() && !user.IsRegistered())
        {
            var subscribedString = passport.ValidateSubscriberInfo(value, user.GetSupportPackage().GetCredentials().GetIssuedAt());
            int.TryParse(subscribedString, out var subscribed);
            if ((subscribed & 1) == 1) ((ApolloUser)user).GetProfile().Registered = true;
        }
        else if (name == "MSNPROFILE" && user.IsAuthenticated() && !user.IsRegistered())
        {
            int.TryParse(value, out var profileCode);
            ((ApolloUser)user).GetProfile().SetProfileCode(profileCode);
        }
    }
}