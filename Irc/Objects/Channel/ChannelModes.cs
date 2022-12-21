using Irc.Constants;
using Irc.Interfaces;
using Irc.Modes.Channel;
using Irc.Modes.Channel.Member;

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
        modes.Add(Resources.MemberModeHost, new Operator());
        modes.Add(Resources.MemberModeVoice, new Voice());
        modes.Add(Resources.ChannelModePrivate, new Private());
        modes.Add(Resources.ChannelModeSecret, new Secret());
        modes.Add(Resources.ChannelModeInvite, new InviteOnly());
        modes.Add(Resources.ChannelModeTopicOp, new TopicOp());
        modes.Add(Resources.ChannelModeNoExtern, new NoExtern());
        modes.Add(Resources.ChannelModeModerated, new Moderated());
        modes.Add(Resources.ChannelModeUserLimit, new UserLimit());
        modes.Add(Resources.ChannelModeBan, new BanList());
        modes.Add(Resources.ChannelModeKey, new Key());

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

    public override string ToString()
    {
        // TODO: <MODESTRING> Fix the below for Limit and Key on mode string
        var limit = modes['l'].Get() > 0 ? $" {modes['l'].Get()}" : string.Empty;
        var key = modes['k'].Get() != 0 ? $" {keypass}" : string.Empty;

        return
            $"{new string(modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}{limit}{key}";
    }
}