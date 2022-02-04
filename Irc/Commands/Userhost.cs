using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;

namespace Irc.Commands
{
    internal class Userhost : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Userhost()
        {
            _requiredMinimumParameters = 0;
        }

        public void Execute(ChatFrame chatFrame)
        {
            if (chatFrame.User.Registered)
            {
                chatFrame.User.Send(Raw.IRCX_RPL_USERHOST_302(chatFrame.Server, chatFrame.User));
            }
            // TODO: What if not registered?
        }
    }
}