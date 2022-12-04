﻿using Irc.Enumerations;

namespace Irc.Extensions.Access.Server;

public class ServerAccess : AccessList
{
    public ServerAccess()
    {
        accessEntries = new Dictionary<EnumAccessLevel, List<AccessEntry>>
        {
            { EnumAccessLevel.OWNER, new List<AccessEntry>() },
            { EnumAccessLevel.HOST, new List<AccessEntry>() },
            { EnumAccessLevel.VOICE, new List<AccessEntry>() },
            { EnumAccessLevel.DENY, new List<AccessEntry>() }
        };
    }
}