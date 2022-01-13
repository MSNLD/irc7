using System;
using System.Collections.Generic;
using Irc.Constants;
using Irc.Extensions.Access;
using Irc.Helpers.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public class UserProperties : PropCollection
{
    public static Prop Oid = new(Resources.UserPropOid, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public static Prop Client = new(Resources.UserPropClient, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public static Prop Ircvers = new(Resources.UserPropIrcvers, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public static Prop MsnProfile = new(Resources.UserPropMsnProfile, Resources.Null, 0, UserAccessLevel.None,
        UserAccessLevel.None, true, false);

    public static Prop MsnRegCookie = new(Resources.UserPropMsnRegCookie, Resources.Null, 256, UserAccessLevel.NoAccess,
        UserAccessLevel.None, true, false);
    
    public static Prop Nick = new(Resources.UserPropNickname, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public static Prop Puid = new(Resources.UserPropPuid, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public static Prop Role = new(Resources.UserPropRole, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public static readonly Dictionary<string, Prop> PropertyRules =
        new Dictionary<string, Prop>(StringComparer.InvariantCultureIgnoreCase)
        {
            { Resources.UserPropOid, Oid },
            { Resources.UserPropClient, Client },
            { Resources.UserPropIrcvers, Ircvers },
            { Resources.UserPropMsnProfile, MsnProfile },
            { Resources.UserPropMsnRegCookie, MsnRegCookie },
            { Resources.UserPropNickname, Nick },
            { Resources.UserPropPuid, Puid },
            { Resources.UserPropRole, Role }
        };

    public UserProperties()
    {
        foreach (string prop in PropertyRules.Keys)
        {
            Set(prop, null);
        }

        Set(Resources.UserPropNickname, Resources.Wildcard);
    }
}