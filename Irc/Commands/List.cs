﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Commands;

internal class List : Command, ICommand
{
    public List() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Data;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var parameters = chatFrame.Message.Parameters;

        var channels = server.GetChannels().Where(c => !c.Modes.Secret).ToList();
        if (parameters.Count > 0)
        {
            var channelNames = parameters.First().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            channels = server
                .GetChannels()
                .Where(c => !c.Modes.Secret
                            && channelNames.Contains(c.GetName(), StringComparer.InvariantCultureIgnoreCase)).ToList();
        }

        ListChannels(server, user, channels);
    }

    public void ListChannels(IServer server, IUser user, IList<IChannel> channels)
    {
        user.Send(Raw.IRCX_RPL_MODE_321(server, user));
        foreach (var channel in channels) user.Send(Raw.IRCX_RPL_MODE_322(server, user, channel));
        user.Send(Raw.IRCX_RPL_MODE_323(server, user));
    }
}