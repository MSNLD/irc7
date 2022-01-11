using CSharpTools;

namespace Core.Authentication;

public class SSPCredentials
{
    public string Username, Password, Nickname, UserGroup, Modes;
    public Core.Ircx.Objects.UserAccessLevel Level;
}