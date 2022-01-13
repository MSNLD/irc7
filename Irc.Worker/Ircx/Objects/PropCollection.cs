using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Irc.Extensions;
using Irc.Extensions.Access;
using Irc.Helpers.CSharpTools;
using Irc.Interfaces;

namespace Irc.Worker.Ircx.Objects;

public class PropCollection: IPropStore
{
    private ConcurrentDictionary<string, string> Properties = new(StringComparer.InvariantCultureIgnoreCase);
    public string Get(string key)
    {
        Properties.TryGetValue(key, out var propertyValue);
        return propertyValue;
    }

    public void Set(string key, string value)
    {
        Properties.AddOrUpdate(key, value, (key, currentValue) => currentValue = value);
    }

    public List<KeyValuePair<string, string>> GetList() => Properties.ToList();
}