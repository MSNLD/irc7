﻿using Irc.Models.Enumerations;

namespace Irc.Extensions.Access.User;

public class UserAccess : AccessList
{
    public UserAccess()
    {
        AccessEntries = new Dictionary<EnumAccessLevel, List<AccessEntry>>
        {
            { EnumAccessLevel.VOICE, new List<AccessEntry>() },
            { EnumAccessLevel.DENY, new List<AccessEntry>() }
        };
    }
}