using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;

namespace Irc.Commands
{
    internal class UserCommand : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;
        public new string GetName() => "User";

        public UserCommand()
        {
            _requiredMinimumParameters = 4;
        }

        public void Execute(ChatFrame chatFrame)
        {
            if (chatFrame.User.Registered) chatFrame.User.Send(Raw.IRCX_ERR_ALREADYREGISTERED_462(chatFrame.Server, chatFrame.User));
            else
            {
                // Gotta check each param
                chatFrame.User.Address.User = chatFrame.Message.Parameters[0];
                //chatFrame.User.Address.Host = chatFrame.Message.Parameters[1];
                chatFrame.User.Address.Host = chatFrame.User.Address.RemoteIP;
                chatFrame.User.Address.Server = chatFrame.Message.Parameters[2];
                chatFrame.User.Address.RealName = chatFrame.Message.Parameters[3];
            }
        }
    }
}