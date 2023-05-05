﻿using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Commands;

internal class Topic : Command, ICommand
{
    public Topic() : base(2) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

    public new void Execute(ChatFrame chatFrame)
    {
        var source = chatFrame.User;
        var channelName = chatFrame.Message.Parameters.First();
        var topic = chatFrame.Message.Parameters[1];

        if (chatFrame.Message.Parameters.Count > 2) topic = chatFrame.Message.Parameters[2];

        var channel = chatFrame.Server.GetChannelByName(channelName);
        if (channel == null) chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, chatFrame.Message.Parameters.First()));
        else
        {
            var result = ProcessTopic(chatFrame, channel, source, topic);
            switch (result)
            {
                case EnumIrcError.ERR_NOTONCHANNEL:
                    {
                        chatFrame.User.Send(Raw.IRCX_ERR_NOTONCHANNEL_442(chatFrame.Server, source, (ChatObject)channel));
                        break;
                    }
                case EnumIrcError.ERR_NOCHANOP:
                    {
                        chatFrame.User.Send(Raw.IRCX_ERR_CHANOPRIVSNEEDED_482(chatFrame.Server, source, (ChatObject)channel));
                        break;
                    }
                case EnumIrcError.OK:
                    {
                        channel.Send(Raw.RPL_TOPIC_IRC(chatFrame.Server, source, channel, topic));
                        break;
                    }
            }
        }
    }

    public static EnumIrcError ProcessTopic(ChatFrame chatFrame, IChannel channel, IUser source, string topic)
    {
        if (!channel.CanBeModifiedBy((ChatObject)source))
        {
            return EnumIrcError.ERR_NOTONCHANNEL;
        }

        IChannelMember sourceMember = channel.GetMember((IUser)source);

        if (sourceMember.GetLevel() < EnumChannelAccessLevel.ChatHost && channel.Modes.TopicOp)
        {
            return EnumIrcError.ERR_NOCHANOP;
        }

        channel.ChannelStore.Set("topic", topic);
        return EnumIrcError.OK;
    }
}