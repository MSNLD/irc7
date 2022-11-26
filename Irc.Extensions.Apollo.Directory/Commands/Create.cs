using Irc.Commands;
using Irc.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo.Directory.Commands
{
    internal class Create : Command, ICommand
    {
        public Create()
        {
            _requiredMinimumParameters = 1;
        }

        public EnumCommandDataType GetDataType()
        {
            return EnumCommandDataType.None;
        }

        public void Execute(ChatFrame chatFrame)
        {
            chatFrame.User.Send(Raw.IRCX_RPL_FINDS_613(chatFrame.Server, chatFrame.User));
        }
    }
}
