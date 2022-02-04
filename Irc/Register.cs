using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc
{
    internal static class Register
    {
        public static void Execute(ChatFrame chatFrame)
        {
            if (CanRegister(chatFrame))
            {
                chatFrame.User.Registered = true;
                chatFrame.User.Send(Raw.IRCX_RPL_WELCOME_001(chatFrame.Server, chatFrame.User));
            }
        }
        public static bool CanRegister(ChatFrame chatFrame)
        {
            bool authenticating = (chatFrame.User.Authenticated != true && chatFrame.User.IsAnon() == false);
            bool registered = chatFrame.User.Registered;
            bool hasNickname = !string.IsNullOrWhiteSpace(chatFrame.User.Address.Nickname);
            bool hasAddress = chatFrame.User.Address.IsAddressPopulated();

            return !authenticating && !registered & hasNickname & hasAddress;
        }

    }
}
