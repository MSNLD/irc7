using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.IO;
using Irc.Objects;

namespace Irc.Extensions.Objects.Channel
{
    public class ExtendedChannel: global::Irc.Objects.Channel.Channel
    {
        public ExtendedChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name, modeCollection, dataStore)
        {
        }
    }
}
