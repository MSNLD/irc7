﻿using Irc.Commands;
using Irc.Enumerations;
using Irc.Interfaces;
using Version = Irc.Commands.Version;

namespace Irc;

public class Irc : Protocol, IProtocol
{
    public Irc()
    {
        AddCommand(new Privmsg());
        AddCommand(new Notice());
        AddCommand(new Ping());
        AddCommand(new Nick());
        AddCommand(new UserCommand(), "User");
        AddCommand(new List());
        AddCommand(new Mode());
        AddCommand(new Join());
        AddCommand(new Part());
        AddCommand(new Kick());
        AddCommand(new Userhost());
        AddCommand(new Version());
        AddCommand(new Info());
        AddCommand(new Pong());
        AddCommand(new Pass());
        AddCommand(new Quit());
        AddCommand(new Trace());
        AddCommand(new Ison());
        AddCommand(new Time());
        AddCommand(new Admin());
        AddCommand(new Links());
        AddCommand(new Whois());
        AddCommand(new Users());
        AddCommand(new Topic());
    }

    public new ICommand GetCommand(string name)
    {
        Commands.TryGetValue(name, out var command);
        return command;
    }

    public new virtual EnumProtocolType GetProtocolType()
    {
        return EnumProtocolType.IRC;
    }

    public override string FormattedUser(IChannelMember member)
    {
        var modeChar = string.Empty;
        if (!member.IsNormal()) modeChar += member.IsOwner() ? '.' : member.IsHost() ? '@' : '+';
        return $"{modeChar}{member.GetUser().GetAddress().Nickname}";
    }
}