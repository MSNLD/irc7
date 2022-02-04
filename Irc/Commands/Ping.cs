using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;

namespace Irc.Commands
{
    internal class Ping : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Ping()
        {
            _requiredMinimumParameters = 1;
        }

        public void Execute(ChatFrame chatFrame)
        {
           chatFrame.User.Send($"PONG :{chatFrame.Message.Parameters.First()}");
        }
    }
}
