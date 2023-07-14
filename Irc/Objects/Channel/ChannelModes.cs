using Irc.Constants;
using Irc.Interfaces;
using Irc.Modes.Channel;
using Irc.Modes.Channel.Member;

namespace Irc.Objects;

public class ChannelModes : ModeCollection, IChannelModes
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
        modes.Add(Resources.ChannelModeHidden, new Hidden());
        modes.Add(Resources.ChannelModeInvite, new InviteOnly());
        modes.Add(Resources.ChannelModeTopicOp, new TopicOp());
        modes.Add(Resources.ChannelModeNoExtern, new NoExtern());
        modes.Add(Resources.ChannelModeModerated, new Moderated());
        modes.Add(Resources.ChannelModeUserLimit, new UserLimit());
        modes.Add(Resources.ChannelModeBan, new BanList());
        modes.Add(Resources.ChannelModeKey, new Key());
    }

    public bool InviteOnly
    {
        get => modes[Resources.ChannelModeInvite].Get() == 1;
        set => modes[Resources.ChannelModeInvite].Set(Convert.ToInt32(value));
    }

    public string Key
    {
        get => keypass;
        set
        {
            var hasKey = !string.IsNullOrWhiteSpace(value);
            modes[Resources.ChannelModeKey].Set(hasKey);
            keypass = value;
        }
    }

    public bool Moderated
    {
        get => modes[Resources.ChannelModeModerated].Get() == 1;
        set => modes[Resources.ChannelModeModerated].Set(Convert.ToInt32(value));
    }

    public bool NoExtern
    {
        get => modes[Resources.ChannelModeNoExtern].Get() == 1;
        set => modes[Resources.ChannelModeNoExtern].Set(Convert.ToInt32(value));
    }

    public bool Private
    {
        get => modes[Resources.ChannelModePrivate].Get() == 1;
        set => modes[Resources.ChannelModePrivate].Set(Convert.ToInt32(value));
    }

    public bool Secret
    {
        get => modes[Resources.ChannelModeSecret].Get() == 1;
        set => modes[Resources.ChannelModeSecret].Set(Convert.ToInt32(value));
    }

    public bool Hidden
    {
        get => modes[Resources.ChannelModeHidden].Get() == 1;
        set => modes[Resources.ChannelModeHidden].Set(Convert.ToInt32(value));
    }

    public bool TopicOp
    {
        get => modes[Resources.ChannelModeTopicOp].Get() == 1;
        set => modes[Resources.ChannelModeTopicOp].Set(Convert.ToInt32(value));
    }

    public int UserLimit
    {
        get => modes[Resources.ChannelModeUserLimit].Get();
        set => modes[Resources.ChannelModeUserLimit].Set(value);
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