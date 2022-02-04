using Irc.Constants;

namespace Irc.Objects;

public class UserModes : ModeCollection, IModeCollection
{
    public UserModes()
    {
        modes.Add(Resources.UserModeAdmin, 0);
        modes.Add(Resources.UserModeOper, 0);
        modes.Add(Resources.UserModeInvisible, 0);
        //modes.Add(Resources.UserModeServerNotice, 0);
        //modes.Add(Resources.UserModeWallops, 0);
    }
}