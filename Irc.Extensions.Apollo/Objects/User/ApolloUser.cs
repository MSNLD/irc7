﻿using Irc.Extensions.Apollo.Objects.Server;
using Irc.Extensions.Objects.User;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Server;
using Irc7d;

namespace Irc.Extensions.Apollo.Objects.User;

public class ApolloUser : ExtendedUser
{
    ApolloProfile Profile { get; set; } = new ApolloProfile();

    public ApolloUser(IConnection connection, IProtocol protocol, IDataRegulator dataRegulator,
        IFloodProtectionProfile floodProtectionProfile, IDataStore dataStore, IModeCollection modes, IServer server) :
        base(connection, protocol, dataRegulator, floodProtectionProfile, dataStore, modes, server)
    {
        _properties = new ApolloUserPropCollection((ApolloServer)server, dataStore);
    }

    public override void PromoteToAdministrator()
    {
        Profile.Level = Enumerations.EnumUserAccessLevel.Administrator;
        base.PromoteToAdministrator();
    }

    public override void PromoteToSysop()
    {
        Profile.Level = Enumerations.EnumUserAccessLevel.Sysop;
        base.PromoteToSysop();
    }

    public override void PromoteToGuide()
    {
        Profile.Level = Enumerations.EnumUserAccessLevel.Guide;
        base.PromoteToGuide();
    }

    public override void SetAway(IServer server, IUser user, string message) {
        Profile.Away = true;
        base.SetAway(server, user, message);
    }

    public override void SetBack(IServer server, IUser user) {
        Profile.Away = false;
        base.SetBack(server, user);
    }

    public ApolloProfile GetProfile() => Profile;
}