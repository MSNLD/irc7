using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;

namespace Irc.Commands
{
    internal class Privmsg : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

        public Privmsg()
        {
            _requiredMinimumParameters = 2;
        }

        public void Execute(ChatFrame chatFrame)
        {
            var channelName = chatFrame.Message.Parameters.First();
            var message = chatFrame.Message.Parameters[1];

            var channel = chatFrame.Server.GetChannelByName(channelName);

            if (channel != null)
            {
                channel.SendMessage(chatFrame.User, message);
            }
            else
            {
                // TODO: To make common function for this
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
            }
        }
    }
}