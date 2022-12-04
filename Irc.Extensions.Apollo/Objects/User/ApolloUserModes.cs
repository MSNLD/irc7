﻿using Irc.Extensions.Apollo.Modes.User;
using Irc.Extensions.Objects.User;

namespace Irc.Extensions.Apollo.Objects.User;

public class ApolloUserModes : ExtendedUserModes
{
    public ApolloUserModes()
    {
        modes.Add(ApolloResources.UserModeHost, new Host());
    }
}