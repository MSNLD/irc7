using Irc.Constants;
using Irc.Modes.User;

namespace Irc.Objects.User;

public class UserModes : ModeCollection
{
    public UserModes()
    {
        modes.Add(Resources.UserModeOper, new Oper());
        modes.Add(Resources.UserModeInvisible, new Invisible());
        //modes.Add(Resources.UserModeServerNotice, new Modes.User.ServerNotice());
        //modes.Add(Resources.UserModeWallops, new Modes.User.WallOps());
    }
}