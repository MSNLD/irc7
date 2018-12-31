using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    class TOPIC : Command
    {

        public TOPIC(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
            base.ForceFloodCheck = true;
        }

        public new COM_RESULT Execute(Frame Frame)
        {
            UserChannelInfo uci = Frame.User.GetChannelInfo(Frame.Message.Data[0]);
            if (uci != null)
            {
                if (Flood.FloodCheck(base.DataType, uci) == FLD_RESULT.S_WAIT) { return COM_RESULT.COM_WAIT; }

                Channel c = uci.Channel;
                ChannelMember cm = c.GetMember(Frame.User);
                if (Frame.Message.Data.Count == 1)
                {
                    if (cm != null)
                    {
                        //give topic 332
                        SendTopicReply(Frame.Server, Frame.User, c);
                    }
                    else if ((c.Modes.Private.Value == 0) && (c.Modes.Secret.Value == 0))
                    {
                        SendTopicReply(Frame.Server, Frame.User, c);
                        //give topic 332
                    }
                    else
                    {
                        //331 No topic is set
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: c, Client: Frame.User, Raw: Raws.IRCX_RPL_NOTOPIC_331));
                    }
                }
                else if (Frame.Message.Data.Count >= 2)
                {
                    if (cm != null)
                    {
                        //give topic
                        bool canChange = false;
                        if (cm.Level >= UserAccessLevel.ChatHost) { canChange = true; }
                        else if (c.Modes.TopicOp.Value == 0) { canChange = true; }

                        if (canChange)
                        {
                            if (Frame.Message.Data[1].length <= c.Properties.Topic.Limit)
                            {
                                c.Properties.Topic.Value = Frame.Message.Data[1];
                                c.Properties.TopicLastChanged = ((DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond);
                                c.Send(Raws.Create(Server: Frame.Server, Channel: c, Client: Frame.User, Raw: Raws.RPL_TOPIC_IRC, Data: new String8[] { c.Properties.Topic.Value }), Frame.User);
                            }
                            else
                            {
                                //421 String parameter must be 160 chars or less
                                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_UNKNOWNCOMMAND_421_T, Data: new String8[] { Resources.CommandTopic }));
                            }
                        }
                        else
                        {
                            //482 You're not channel operator
                            Frame.User.Send(Raws.Create(Server: Frame.Server, Channel: c, Client: Frame.User, Raw: Raws.IRCX_ERR_CHANOPRIVSNEEDED_482));
                        }
                    }
                    else
                    {
                        //442 You are not on that channel
                        Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new String8[] { Frame.Message.Data[0] }));
                    }
                }
            }
            return COM_RESULT.COM_SUCCESS;
        }

        public static void SendTopicReply(Server server, User user, Channel c)
        {
            if (c.Properties.Topic.Value.length > 0)
            {
                user.Send(Raws.Create(Server: server, Channel: c, Client: user, Raw: Raws.IRCX_RPL_TOPIC_332, Data: new String8[] { c.Properties.Topic.Value }));
            }
        }
    }
}
