using Core.Ircx.Objects;

namespace Core.Authentication;

public class SSPCredentials
{
    public UserAccessLevel Level;
    public string Username, Password, Nickname, UserGroup, Modes;
}