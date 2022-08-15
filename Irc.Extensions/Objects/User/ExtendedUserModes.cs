using Irc.Constants;
using Irc.Objects;

namespace Irc.Extensions.Objects.User;

public class ExtendedUserModes : UserModes
{
    public ExtendedUserModes()
    {
        modes.Add(Resources.UserModeAdmin, new Modes.User.Admin());
        modes.Add(ExtendedResources.UserModeIrcx, new Modes.User.Isircx());
        modes.Add(ExtendedResources.UserModeGag, new Modes.User.Gag());
    }
}