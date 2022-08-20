using Irc.Extensions.Interfaces;
using Irc.Extensions.Props;
using Irc.IO;

namespace Irc.Objects.Collections;

public class PropCollection : IPropCollection
{
    protected Dictionary<string, PropRule> properties = new Dictionary<string, PropRule>();

    public PropCollection()
    {

    }

    public void AddProp(PropRule prop)
    {
        properties[prop.Name.ToUpper()] = prop;
    }

    public IPropRule GetProp(string name)
    {
        properties.TryGetValue(name, out var rule);
        return rule;
    }

    public void SetProp(string name, string value)
    {
        properties[name].SetValue(value);
    }
}