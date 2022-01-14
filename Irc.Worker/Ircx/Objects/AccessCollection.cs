using System.Collections.Generic;

namespace Irc.Worker.Ircx.Objects;

public class AccessCollection
{
    public List<AccessEntry> Entries;

    public AccessCollection()
    {
        Entries = new List<AccessEntry>();
    }

    public AccessEntry Contains(Address QueryMask)
    {
        for (var i = 0; i < Entries.Count; i++)
            if (QueryMask.GetFullAddress() == Entries[i].Mask.GetFullAddress())
                return Entries[i];
        return null;
    }

    public void Add(AccessEntry Entry)
    {
        Entries.Add(Entry);
    }

    public void Remove(AccessEntry Entry)
    {
        Entries.Remove(Entry);
    }
}