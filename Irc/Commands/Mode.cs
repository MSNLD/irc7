using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Worker.Ircx.Objects;

namespace Irc.Commands
{
    internal class Mode : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Mode()
        {
            _requiredMinimumParameters = 1;
        }

        public void Execute(ChatFrame chatFrame)
        {
            if (!chatFrame.User.Registered)
            {
                if (chatFrame.Message.Parameters.First().ToUpper() == Resources.ISIRCX)
                {
                    var protocol = chatFrame.User.GetProtocol().GetProtocolType();
                    bool isircx = (protocol > EnumProtocolType.IRC);
                    chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0, chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
                }
            }

        }
    }
}