using Irc.Enumerations;

namespace Irc.Extensions.Access.User;

public class UserAccess : AccessList
{
    public UserAccess()
    {
        accessEntries = new Dictionary<EnumAccessLevel, List<AccessEntry>>
        {
            { EnumAccessLevel.VOICE, new List<AccessEntry>() },
            { EnumAccessLevel.DENY, new List<AccessEntry>() }
        };
    }
}