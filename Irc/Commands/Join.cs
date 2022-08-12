﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;

namespace Irc.Commands;

internal class Join : Command, ICommand
{
    public Join() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var channels = chatFrame.Message.Parameters.First();
        var key = chatFrame.Message.Parameters.Count > 1 ? chatFrame.Message.Parameters[1] : string.Empty;

        var channelNames = ValidateChannels(server, user, channels);
        if (channelNames.Count == 0) return;

        JoinChannels(server, user, channelNames, key);
    }

    public static List<string> ValidateChannels(IServer server, IUser user, string channels)
    {
        var channelNames = channels.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (channelNames.Count == 0)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, string.Empty));
        }
        else
        {
            var invalidChannelNames = channelNames.Where(c => !Channel.ValidName(c)).ToList();
            channelNames.RemoveAll(c => invalidChannelNames.Contains(c));

            // TODO: Could do better below for reporting invalid channel / empty channel
            if (invalidChannelNames.Count > 0)
                invalidChannelNames.ForEach(c => user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, c)));
        }

        return channelNames;
    }

    public void JoinChannels(IServer server, IUser user, List<string> channelNames, string key)
    {
        // TODO: Optimize the below code
        foreach (var channelName in channelNames)
        {
            var channel = server
                            .GetChannels()
                                .FirstOrDefault(c => c.GetName().ToUpper() == channelName.ToUpper());

            if (channel == null) channel = Create(server, user, channelName, key);
            else if (!channel.Allows(user))
            {
                user.Send("CANNOT JOIN CHANNEL");
                continue;
            }

            channel.Join(user)
            .SendTopic(user)
            .SendNames(user);
        }
    }

    public IChannel Create(IServer server, IUser user, string name, string key)
    {
        var channel = server.CreateChannel(name);
        channel.ChannelStore.Set("topic", name);
        channel.ChannelStore.Set("key", key);
        channel.GetModes().SetModeChar('n', 1);
        channel.GetModes().SetModeChar('t', 1);
        channel.GetModes().SetModeChar('l', 50);
        server.AddChannel(channel);
        return channel;
    }
}