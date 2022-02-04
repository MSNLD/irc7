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
    internal class Ircx : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Ircx()
        {
            _requiredMinimumParameters = 0;
        }

        public void Execute(ChatFrame chatFrame)
        {
            var protocol = chatFrame.User.GetProtocol().GetProtocolType();
            if (protocol < EnumProtocolType.IRC0)
            {
                protocol = EnumProtocolType.IRCX;
                chatFrame.User.SetProtocol(chatFrame.Server.GetProtocol(protocol));

            }
            bool isircx = (protocol > EnumProtocolType.IRC);

            chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
                chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
        }
    }
}