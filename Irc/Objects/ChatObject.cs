using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.IO;

namespace Irc.Objects;

public class ChatObject : IChatObject
{
    public readonly IDataStore DataStore;
    public virtual EnumUserAccessLevel Level
    {
        get
        {
            return EnumUserAccessLevel.None;
        }
    }
    protected readonly IModeCollection _modes;
    public IModeCollection Modes
    {
        get
        {
            return _modes;
        }
    }

    public ChatObject(IModeCollection modes, IDataStore dataStore)
    {
        _modes = modes;
        DataStore = dataStore;
        DataStore.SetId(Id.ToString());
    }

    public IModeCollection GetModes() => _modes;

    public Guid Id { get; } = Guid.NewGuid();

    public string ShortId => Id.ToString().Split('-').Last();

    public string Name
    {
        get => DataStore.Get("Name") ?? Resources.Wildcard;
        set => DataStore.Set("Name", value);
    }

    public virtual void Send(string message)
    {
        throw new NotImplementedException();
    }

    public virtual void Send(string message, ChatObject except = null)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return Name;
    }

    public bool CanBeModifiedBy(ChatObject source)
    {
        throw new NotImplementedException();
    }
}