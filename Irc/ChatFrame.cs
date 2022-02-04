using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Objects;
using Irc.Worker.Ircx;
using Irc.Worker.Ircx.Objects;
using Irc7d;

namespace Irc
{
    public class ChatFrame
    {
        public readonly Server Server;
        public readonly User User;
        public readonly Message Message;

        public ChatFrame(Server server, User user, Message message)
        {
            Server = server;
            User = user;
            Message = message;
        }
    }
}
