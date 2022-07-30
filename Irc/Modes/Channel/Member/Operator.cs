using Irc.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel.Member
{
    public class Operator : ModeRule
    {
        public Operator() : base('o')
        {
        }
    }
}
