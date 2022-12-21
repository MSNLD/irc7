using Irc.Extensions.Interfaces;

namespace Irc.Extensions.Objects.Collections;

public class PropCollection : IPropCollection
{
    protected readonly Dictionary<string, IPropRule> Properties = new();

    public IPropRule GetProp(string name)
    {
        Properties.TryGetValue(name, out var rule);
        return rule;
    }

    public List<IPropRule> GetProps()
    {
        return Properties.Values.ToList();
    }

    public void AddProp(IPropRule prop)
    {
        Properties[prop.Name.ToUpper()] = prop;
    }

    public void SetProp(string name, string value)
    {
        Properties[name].SetValue(value);
    }
}