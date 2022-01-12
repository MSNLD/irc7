using System.Collections.Generic;

namespace Irc.Worker.Ircx.Objects;

public class UserChannelCollection
{
    public List<UserChannelInfo> ChannelList { get; } = new();

    public void AddChannelInfo(UserChannelInfo channel)
    {
        ChannelList.Add(channel);
    }

    public void RemChannel(Channel channel)
    {
        for (var c = 0; c < ChannelList.Count; c++)
            if (ChannelList[c].Channel == channel)
            {
                ChannelList.RemoveAt(c);
                return;
            }
    }
}