using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Commands;
using Irc.Extensions.Protocols;

namespace Irc.Extensions.Apollo.Protocols
{
    public class Irc3: IrcX
    {
        public Irc3()
        {
            AddCommand(new Ircvers());
        }
    }
}
