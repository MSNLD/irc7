namespace Irc.Worker.Ircx.Objects;

public class UserChannelInfo
{
    public Channel Channel;
    public ChannelMember Member;

    public UserChannelInfo(Channel Channel, ChannelMember Member)
    {
        this.Channel = Channel;
        this.Member = Member;
    }
}