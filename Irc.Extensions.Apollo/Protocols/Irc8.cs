﻿using Irc.Extensions.Apollo.Objects.User;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Protocols;

internal class Irc8 : Irc7
{
    public override string FormattedUser(IChannelMember member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';

        var profile = ((ApolloUser)member.GetUser()).GetProfile().ToString();
        return $"{profile},{modeChar}{member.GetUser().GetAddress().Nickname}";
    }

    public override EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC8;
    }

    public override string GetFormat(IUser user)
    {
        return ((ApolloUser)user).GetProfile().ToString();
    }
}