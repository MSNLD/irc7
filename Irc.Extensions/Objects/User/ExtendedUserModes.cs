using Irc.Objects;

namespace Irc.Extensions.Objects.User;

public class ExtendedUserModes : UserModes
{
    public ExtendedUserModes()
    {
        modes.Add(ExtendedResources.UserModeIrcx, 0);
        modes.Add(ExtendedResources.UserModeGag, 0);
    }
}