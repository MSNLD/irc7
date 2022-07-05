using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc7d
{
    public class DefaultChannel
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public Dictionary<char, int> Modes { get; set; } = new Dictionary<char, int>();
    }
}
