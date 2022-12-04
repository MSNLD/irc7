using Irc.Extensions.Props.User;
using Irc.IO;
using Irc.Objects.Collections;

namespace Irc.Extensions.Objects.User;

internal class UserPropCollection : PropCollection
{
    private readonly IDataStore dataStore;

    public UserPropCollection(IDataStore dataStore)
    {
        AddProp(new OID(dataStore));
        AddProp(new Nick(dataStore));
        this.dataStore = dataStore;
    }
}