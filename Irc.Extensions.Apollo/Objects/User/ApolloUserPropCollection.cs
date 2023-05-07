using System;
using Irc.Extensions.Apollo.Objects.Server;
using Irc.Extensions.Apollo.Props.User;
using Irc.Extensions.Objects.User;
using Irc.IO;

namespace Irc.Extensions.Apollo.Objects.User
{
	public class ApolloUserPropCollection: UserPropCollection
    {
		public ApolloUserPropCollection(ApolloServer apolloServer, IDataStore dataStore) : base(dataStore)
		{
			AddProp(new SubscriberInfo(apolloServer));
			//AddProp(new Msnprofile());
		}
	}
}

