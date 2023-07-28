using Irc.Constants;
using Irc.Modes.User;

namespace Irc.Objects;

public class UserModes : ModeCollection, IModeCollection
{
    public UserModes()
    {
        modes.Add(Resources.UserModeOper, new Oper());
        modes.Add(Resources.UserModeInvisible, new Invisible());
        modes.Add(Resources.UserModeSecure, new Secure());
        //modes.Add(Resources.UserModeServerNotice, new Modes.User.ServerNotice());
        //modes.Add(Resources.UserModeWallops, new Modes.User.WallOps());
    }

    public bool Oper
    {
        get => modes[Resources.UserModeOper].Get() == 1;
        set => modes[Resources.UserModeOper].Set(Convert.ToInt32(value));
    }

    public bool Invisible
    {
        get => modes[Resources.UserModeInvisible].Get() == 1;
        set => modes[Resources.UserModeInvisible].Set(Convert.ToInt32(value));
    }

    public bool Secure
    {
        get => modes[Resources.UserModeSecure].Get() == 1;
        set => modes[Resources.UserModeSecure].Set(Convert.ToInt32(value));
    }
}