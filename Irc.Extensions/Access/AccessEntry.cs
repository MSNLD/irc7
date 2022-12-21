using Irc.Models.Enumerations;

namespace Irc.Extensions.Access;

public class AccessEntry
{
    public AccessEntry(string entryAddress, EnumUserAccessLevel entryLevel, EnumAccessLevel accessLevel, string mask,
        int timeout, string reason)
    {
        EntryAddress = entryAddress;
        EntryLevel = entryLevel;
        AccessLevel = accessLevel;
        Mask = mask;
        Timeout = timeout;
        Reason = reason;
    }

    public DateTime Creation { get; } = DateTime.UtcNow;
    public string EntryAddress { get; }
    public EnumUserAccessLevel EntryLevel { get; }
    public EnumAccessLevel AccessLevel { get; }
    public string Mask { get; }
    public int Timeout { get; }
    public string Reason { get; }
}