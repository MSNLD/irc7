using Irc.Enumerations;

namespace Irc.Extensions.Security;

public class Credential : ICredential
{
    public string Domain { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Nickname { get; set; }
    public string UserGroup { get; set; }
    public string Modes { get; set; }
    public EnumUserAccessLevel Level { get; set; }

    public string GetDomain()
    {
        return Domain;
    }

    public string GetUsername()
    {
        return Username;
    }

    public string GetPassword()
    {
        return Password;
    }

    public string GetNickname()
    {
        return Nickname;
    }

    public string GetUserGroup()
    {
        return UserGroup;
    }

    public string GetModes()
    {
        return Modes;
    }

    public EnumUserAccessLevel GetLevel()
    {
        return Level;
    }
}