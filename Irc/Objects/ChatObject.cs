using Irc.Constants;
using Irc.Enumerations;
using Irc.IO;

namespace Irc.Objects;

public class ChatObject
{
    public readonly IDataStore DataStore;
    public EnumUserAccessLevel Level = EnumUserAccessLevel.None;

    public ChatObject(IDataStore dataStore)
    {
        DataStore = dataStore;
        DataStore.SetId(Id.ToString());
    }

    public Guid Id { get; } = Guid.NewGuid();

    public string ShortId => Id.ToString().Split('-').Last();

    public string Name
    {
        get => DataStore.Get("Name") ?? Resources.Wildcard;
        set => DataStore.Set("Name", value);
    }

    public override string ToString()
    {
        return Name;
    }
}