using Irc.Constants;
using Irc.Interfaces;

namespace Irc.Objects;

public class ChatObject
{
    private readonly Guid _id = Guid.NewGuid();
    public Guid Id => _id;
    public string ShortId => Id.ToString().Split('-').Last();
    public IPropStore Properties;

    public ChatObject(IPropStore properties)
    {
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