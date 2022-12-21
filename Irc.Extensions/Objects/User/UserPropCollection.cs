using Irc.Extensions.Objects.Collections;
using Irc.Extensions.Props.User;
using Irc.Interfaces;
using Irc.IO;

namespace Irc.Extensions.Objects.User;

internal class UserPropCollection : PropCollection
{
    private readonly IDataStore _dataStore;

    public UserPropCollection(IDataStore dataStore)
    {
        AddProp(new Oid(dataStore));
        AddProp(new Nick(dataStore));
        this._dataStore = dataStore;
    }
}