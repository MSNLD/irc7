using System;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx.Commands;

internal class TOPIC : Command
{
    public TOPIC(CommandCode Code) : base(Code)
    {
        MinParamCount = 1;
        RegistrationRequired = true;
        DataType = CommandDataType.Standard;
        ForceFloodCheck = true;
    }

    public new bool Execute(Frame Frame)
    {
        var channelMemberPair = Frame.User.GetChannelInfo(Frame.Message.Parameters[0]);
        var channel = channelMemberPair.Key;
        var channelMember = channelMemberPair.Value;
        if (channel != null)
        {
            if (Flood.FloodCheck(DataType, channelMember.User) == FLD_RESULT.S_WAIT) return false;

            var cm = channel.GetMember(Frame.User);
            if (Frame.Message.Parameters.Count == 1)
            {
                if (cm != null)
                    //give topic 332
                    SendTopicReply(Frame.Server, Frame.User, channel);
                else if (channel.Modes.Private.Value == 0 && channel.Modes.Secret.Value == 0)
                    SendTopicReply(Frame.Server, Frame.User, channel);
                //give topic 332
                else
                    //331 No topic is set
                    Frame.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.IRCX_RPL_NOTOPIC_331));
            }
            else if (Frame.Message.Parameters.Count >= 2)
            {
                if (cm != null)
                {
                    //give topic
                    var canChange = false;
                    if (cm.Level >= UserAccessLevel.ChatHost)
                        canChange = true;
                    else if (channel.Modes.TopicOp.Value == 0) canChange = true;

                    if (canChange)
                    {
                        if (Frame.Message.Parameters[1].Length <= ChannelProperties.PropertyRules["Topic"].Limit)
                        {
                            channel.Properties.Set("Topic", Frame.Message.Parameters[1]);
                            channel.TopicLastChanged =
                                (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;
                            channel.Send(
                                RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.RPL_TOPIC_IRC,
                                    new[] {channel.Properties.Get("Topic")}), Frame.User);
                        }
                        else
                        {
                            //421 String parameter must be 160 chars or less
                            Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421_T, Data: new[] {Resources.CommandTopic}));
                        }
                    }
                    else
                    {
                        //482 You're not channel operator
                        Frame.User.Send(RawBuilder.Create(Frame.Server, channel, Frame.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                    }
                }
                else
                {
                    //442 You are not on that channel
                    Frame.User.Send(RawBuilder.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                        Data: new[] {Frame.Message.Parameters[0]}));
                }
            }
        }

        return true;
    }

    public static void SendTopicReply(Server server, User user, Channel c)
    {
        if (c.Properties.Get("Topic")?.Length > 0)
            user.Send(RawBuilder.Create(server, c, user, Raws.IRCX_RPL_TOPIC_332, new[] {c.Properties.Get("Topic")}));
    }
}