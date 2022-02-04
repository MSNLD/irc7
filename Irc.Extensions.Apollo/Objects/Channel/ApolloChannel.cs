using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Objects.Channel;
using Irc.IO;
using Irc.Objects;

namespace Irc.Extensions.Apollo.Objects.Channel
{
    public class ApolloChannel: ExtendedChannel
    {
        public ApolloChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name, modeCollection, dataStore)
        {
        }
    }
}
