﻿using Irc.Extensions.Apollo.Interfaces;
using Irc.Extensions.Apollo.Modes.Channel;
using Irc.Extensions.Modes.Channel;
using Irc.Extensions.Objects.Channel;

namespace Irc.Extensions.Apollo.Objects.Channel;

public class ApolloChannelModes : ExtendedChannelModes, IApolloChannelModes
{
    public ApolloChannelModes()
    {
        modes.Add(ExtendedResources.ChannelModeNoGuestWhisper, new NoGuestWhisper());
        modes.Add(ApolloResources.ChannelModeOnStage, new OnStage());
        modes.Add(ApolloResources.ChannelModeSubscriber, new Subscriber());
    }

    public bool OnStage
    {
        get => modes[ApolloResources.ChannelModeOnStage].Get() == 1;
        set => modes[ApolloResources.ChannelModeOnStage].Set(Convert.ToInt32(value));
    }

    public bool Subscriber
    {
        get => modes[ApolloResources.ChannelModeSubscriber].Get() == 1;
        set => modes[ApolloResources.ChannelModeSubscriber].Set(Convert.ToInt32(value));
    }
}