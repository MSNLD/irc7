using Irc.Extensions.Access;

namespace Irc.Worker.Ircx.Objects;

public class UserProperties : PropCollection
{
    public Prop Client = new(Resources.UserPropClient, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop Ircvers = new(Resources.UserPropIrcvers, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop MsnProfile = new(Resources.UserPropMsnProfile, Resources.Null, 0, UserAccessLevel.None,
        UserAccessLevel.None, true, false);

    public Prop MsnRegCookie = new(Resources.UserPropMsnRegCookie, Resources.Null, 256, UserAccessLevel.NoAccess,
        UserAccessLevel.None, true, false);

    // User Properties
    public Prop Nick = new(Resources.UserPropNickname, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None,
        true, false);

    public Prop Puid = new(Resources.UserPropPuid, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public Prop Role = new(Resources.UserPropRole, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess,
        true, false);

    public UserProperties(Client obj) : base(obj)
    {
        Properties.Add(Nick);
        Properties.Add(Ircvers);
        Properties.Add(Client);
        Properties.Add(MsnProfile);
        Properties.Add(MsnRegCookie);
        Properties.Add(Role);
        Properties.Add(Puid);
    }
}