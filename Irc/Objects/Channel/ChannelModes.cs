using Irc.Constants;

namespace Irc.Objects;

public class ChannelModes : ModeCollection, IModeCollection
{
    public ChannelModes()
    {
        modes.Add(Resources.ChannelModeKey, 0);
        modes.Add(Resources.ChannelModeInvite, 0);
        modes.Add(Resources.ChannelModeUserLimit, 0);
        modes.Add(Resources.ChannelModeModerated, 0);
        modes.Add(Resources.ChannelModeNoExtern, 0);
        modes.Add(Resources.ChannelModePrivate, 0);
        modes.Add(Resources.ChannelModeSecret, 0);
        modes.Add(Resources.ChannelModeTopicOp, 0);
        modes.Add(Resources.ChannelModeBan, 0);
    }
}