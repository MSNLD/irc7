using Irc.IO;

namespace Irc.Objects.Collections;

public class PropCollection : DataStore, IDataStore
{
    public PropCollection(string id) : base(id, "properties")
    {
    }
}