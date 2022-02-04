using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Commands;
using Irc.Worker.Ircx.Objects;

namespace Irc
{
    public class Protocol : IProtocol
    {
        protected Dictionary<string, ICommand> Commands = new(StringComparer.InvariantCultureIgnoreCase);

        public ICommand GetCommand(string name)
        {
            throw new NotImplementedException();
        }

        public EnumProtocolType GetProtocolType()
        {
            throw new NotImplementedException();
        }
    }
}
