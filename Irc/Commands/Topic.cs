using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

internal class Topic : Command, ICommand
{
    public Topic() : base(2)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        var source = chatFrame.User;
        var channelName = chatFrame.Message.Parameters.First();
        var topic = chatFrame.Message.Parameters[1];

        var channel = chatFrame.Server.GetChannelByName(channelName);
        if (channel == null)
        {
            chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User,
                chatFrame.Message.Parameters.First()));
        }
        else
        {
            if (!source.IsOn(channel))
            {
                chatFrame.User.Send(Raw.IRCX_ERR_NOTONCHANNEL_442(chatFrame.Server, source, channel));
                return;
            }

            var member = channel.GetMember(source);
            if (member.GetLevel() < EnumChannelAccessLevel.ChatHost &&
                channel.GetModes().HasMode(Resources.ChannelModeTopicOp))
            {
                chatFrame.User.Send(Raw.IRCX_ERR_CHANOPRIVSNEEDED_482(chatFrame.Server, source, channel));
                return;
            }
            
            channel.ChannelStore.Set("topic", topic);
            channel.Send(Raw.RPL_TOPIC_IRC(chatFrame.Server, source, channel, topic));
        }
    }
}