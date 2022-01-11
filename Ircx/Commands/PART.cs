using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Ircx.Objects;
using CSharpTools;
using System.Reflection;

namespace Core.Ircx.Commands
{
    public class PART : Command
    {
        public PART(CommandCode Code) : base(Code)
        {
            base.MinParamCount = 1;
            base.RegistrationRequired = true;
            base.DataType = CommandDataType.Standard;
        }

        public static void ProcessPartChannel(Frame Frame, Channel c)
        {
            if (c.Contains(Frame.User))
            {
                c.Send(Raws.Create(Server: Frame.Server, Channel: c, Client: Frame.User, Raw: Raws.RPL_PART_IRC), Frame.User);
                c.RemoveMember(Frame.User);

                Frame.User.RemoveChannel(c);
            }
            else
            {
                // you are not on that channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOTONCHANNEL_442, Data: new string[] { c.Name }));
            }
        }
        public static void ProcessPartUserChannels(Frame Frame)
        {
            for (int c = 0; c < Frame.User.ChannelList.Count; c++)
            {
                ProcessPartChannel(Frame, Frame.User.ChannelList[c].Channel); c--;
            }

        }

        public new COM_RESULT Execute(Frame Frame)
        {
            Message message = Frame.Message;

            if (message.Data.Count >= 1)
            {
                // To process 0
                if ((message.Data[0] == Resources.Zero) && (message.Data.Count == 1)) { ProcessPartUserChannels(Frame); return COM_RESULT.COM_SUCCESS; }

                List<Channel> channels = Frame.Server.Channels.GetChannels(Frame.Server, Frame.User, message.Data[0], true);

                if (channels != null)
                {
                    if (channels.Count > 0)
                    {
                        for (int c = 0; c < channels.Count; c++)
                        {
                            ProcessPartChannel(Frame, channels[c]);
                        }
                        return COM_RESULT.COM_SUCCESS;
                    }
                }

                // null
                // No such channel
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NOSUCHCHANNEL_403, Data: new string[] { Frame.Message.Data[0] }));
            }
            else
            {
                //insufficient parameters
                Frame.User.Send(Raws.Create(Server: Frame.Server, Client: Frame.User, Raw: Raws.IRCX_ERR_NEEDMOREPARAMS_461, Data: new string[] { message.Data[0] }));
            }
            return COM_RESULT.COM_SUCCESS;
        }
    }
}
