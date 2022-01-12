using System;
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

    public new COM_RESULT Execute(Frame Frame)
    {
        var uci = Frame.User.GetChannelInfo(Frame.Message.Data[0]);
        if (uci != null)
        {
            if (Flood.FloodCheck(DataType, uci) == FLD_RESULT.S_WAIT) return COM_RESULT.COM_WAIT;

            var c = uci.Channel;
            var cm = c.GetMember(Frame.User);
            if (Frame.Message.Data.Count == 1)
            {
                if (cm != null)
                    //give topic 332
                    SendTopicReply(Frame.Server, Frame.User, c);
                else if (c.Modes.Private.Value == 0 && c.Modes.Secret.Value == 0)
                    SendTopicReply(Frame.Server, Frame.User, c);
                //give topic 332
                else
                    //331 No topic is set
                    Frame.User.Send(Raws.Create(Frame.Server, c, Frame.User, Raws.IRCX_RPL_NOTOPIC_331));
            }
            else if (Frame.Message.Data.Count >= 2)
            {
                if (cm != null)
                {
                    //give topic
                    var canChange = false;
                    if (cm.Level >= UserAccessLevel.ChatHost)
                        canChange = true;
                    else if (c.Modes.TopicOp.Value == 0) canChange = true;

                    if (canChange)
                    {
                        if (Frame.Message.Data[1].Length <= c.Properties.Topic.Limit)
                        {
                            c.Properties.Topic.Value = Frame.Message.Data[1];
                            c.Properties.TopicLastChanged =
                                (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;
                            c.Send(
                                Raws.Create(Frame.Server, c, Frame.User, Raws.RPL_TOPIC_IRC,
                                    new[] {c.Properties.Topic.Value}), Frame.User);
                        }
                        else
                        {
                            //421 String parameter must be 160 chars or less
                            Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User,
                                Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421_T, Data: new[] {Resources.CommandTopic}));
                        }
                    }
                    else
                    {
                        //482 You're not channel operator
                        Frame.User.Send(Raws.Create(Frame.Server, c, Frame.User, Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                    }
                }
                else
                {
                    //442 You are not on that channel
                    Frame.User.Send(Raws.Create(Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442,
                        Data: new[] {Frame.Message.Data[0]}));
                }
            }
        }

        return COM_RESULT.COM_SUCCESS;
    }

    public static void SendTopicReply(Server server, User user, Channel c)
    {
        if (c.Properties.Topic.Value.Length > 0)
            user.Send(Raws.Create(server, c, user, Raws.IRCX_RPL_TOPIC_332, new[] {c.Properties.Topic.Value}));
    }
}