using Irc.Constants;

namespace Irc.Objects;

public class UserModes : ModeCollection, IModeCollection
{
    public UserModes()
    {
        modes.Add(Resources.UserModeOper, new Modes.User.Oper());
        modes.Add(Resources.UserModeInvisible, new Modes.User.Invisible());
        //modes.Add(Resources.UserModeServerNotice, new Modes.User.ServerNotice());
        //modes.Add(Resources.UserModeWallops, new Modes.User.WallOps());
    }
}