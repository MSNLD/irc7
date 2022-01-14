using System.Collections.Generic;
using Irc.Constants;

namespace Irc.Worker.Ircx.Objects;

public class AccessLevelCollection
{
    public List<AccessLevel> Levels;

    public AccessLevelCollection(bool IsChannel)
    {
        Levels = new List<AccessLevel>();
        if (IsChannel)
        {
            Levels.Add(new AccessLevel(EnumAccessLevel.OWNER, Resources.AccessLevelOwner));
            Levels.Add(new AccessLevel(EnumAccessLevel.HOST, Resources.AccessLevelHost));
            Levels.Add(new AccessLevel(EnumAccessLevel.VOICE, Resources.AccessLevelVoice));
        }

        Levels.Add(new AccessLevel(EnumAccessLevel.GRANT, Resources.AccessLevelGrant));
        Levels.Add(new AccessLevel(EnumAccessLevel.DENY, Resources.AccessLevelDeny));
    }
}