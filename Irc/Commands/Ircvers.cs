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
    internal class Ircvers : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

        public Ircvers()
        {
            _requiredMinimumParameters = 2;
        }

        public void Execute(ChatFrame chatFrame)
        {
            if (chatFrame.User.Registered) chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
            else
            {
                string ircvers = chatFrame.Message.Parameters[0].ToUpper();

                if (ircvers.Length == 4 && ircvers.StartsWith("IRC") && char.IsNumber(ircvers.Last()))
                {
                    if (Enum.TryParse<EnumProtocolType>(ircvers, true, out var enumProtocolType ))
                    {
                        if (chatFrame.Server._protocols.TryGetValue(enumProtocolType, out var protocol))
                        {
                            chatFrame.User.SetProtocol(protocol);
                            // TODO: Where exactly to store this needs further consideration
                            chatFrame.User.Client = chatFrame.Message.Parameters[1];

                            bool isircx = (protocol.GetProtocolType() > EnumProtocolType.IRC);
                            chatFrame.User.Send(Raw.IRCX_RPL_IRCX_800(chatFrame.Server, chatFrame.User, isircx ? 1 : 0, 0,
                                chatFrame.Server.MaxInputBytes, Resources.IRCXOptions));
                        }
                        else
                        {
                            chatFrame.User.Send(Raw.IRCX_ERR_BADVALUE_906(chatFrame.Server, chatFrame.User, ircvers));
                        }
                    }
                    return;
                }

                chatFrame.User.Send(Raw.IRCX_ERR_BADVALUE_906(chatFrame.Server, chatFrame.User, ircvers));
            }
        }
    }
}