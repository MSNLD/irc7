using Irc.Constants;
using Irc.Extensions.Modes.User;
using Irc.Objects.User;

namespace Irc.Extensions.Objects.User;

public class ExtendedUserModes : UserModes
{
    public ExtendedUserModes()
    {
        modes.Add(Resources.UserModeAdmin, new Admin());
        modes.Add(ExtendedResources.UserModeIrcx, new Isircx());
        modes.Add(ExtendedResources.UserModeGag, new Gag());
    }
}