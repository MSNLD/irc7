using Irc.Extensions.Access;

namespace Irc.Extensions.Security;

public class Credentials
{
    public UserAccessLevel Level;
    public string Username, Password, Nickname, UserGroup, Modes;
}