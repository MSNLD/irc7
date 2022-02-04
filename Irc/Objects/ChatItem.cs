using Irc.Constants;
using Irc.Interfaces;
using Irc.Worker.Ircx.Objects;

namespace Irc.Objects;

public class ChatItem
{
    private readonly Guid _id = Guid.NewGuid();
    public Guid Id => _id;
    public string ShortId => Id.ToString().Split('-').Last();
    public readonly IPropStore Properties;
    public readonly IObjectStore ObjectStore;

    public ChatItem(IObjectStore objectStore, IPropStore properties)
    {
        ObjectStore = objectStore;
        Properties = properties;
    }

    public string Name
    {
        get => Properties.Get("Name") ?? Resources.Wildcard;
        set => Properties.Set("Name", value);
    }
    public override string ToString()
    {
        return Name;
    }
}