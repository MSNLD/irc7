using Irc.Constants;

namespace Irc.Objects;

public class ChannelModes : ModeCollection, IModeCollection
{
    /*
           o - give/take channel operator privileges;
           p - private channel flag;
           s - secret channel flag;
           i - invite-only channel flag;
           t - topic settable by channel operator only flag;
           n - no messages to channel from clients on the outside;
           m - moderated channel;
           l - set the user limit to channel;
           b - set a ban mask to keep users out;
           v - give/take the ability to speak on a moderated channel;
           k - set a channel key (password).
    */
    public ChannelModes()
    {
        modes.Add(Resources.ChannelModePrivate, 0);
        modes.Add(Resources.ChannelModeSecret, 0);
        modes.Add(Resources.ChannelModeInvite, 0);
        modes.Add(Resources.ChannelModeTopicOp, 0);
        modes.Add(Resources.ChannelModeNoExtern, 0);
        modes.Add(Resources.ChannelModeModerated, 0);
        modes.Add(Resources.ChannelModeUserLimit, 0);
        modes.Add(Resources.ChannelModeBan, 0);
        modes.Add(Resources.ChannelModeKey, 0);

        //modes.Add(Resources.ChannelModeKey, 0);
        //modes.Add(Resources.ChannelModeInvite, 0);
        //modes.Add(Resources.ChannelModeUserLimit, 0);
        //modes.Add(Resources.ChannelModeModerated, 0);
        //modes.Add(Resources.ChannelModeNoExtern, 0);
        //modes.Add(Resources.ChannelModePrivate, 0);
        //modes.Add(Resources.ChannelModeSecret, 0);
        //modes.Add(Resources.ChannelModeTopicOp, 0);
        //modes.Add(Resources.ChannelModeBan, 0);
    }
}