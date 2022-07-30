using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Objects.Channel;
using Irc.Interfaces;
using Irc.IO;
using Irc.Objects;
using Irc.Objects.Member;

namespace Irc.Extensions.Apollo.Objects.Channel;

public class ApolloChannel : ExtendedChannel
{
    public ApolloChannel(string name, IModeCollection modeCollection, IDataStore dataStore) : base(name, modeCollection,
        dataStore)
    {
    }
}