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

    public string GetDomain() => Domain;
    public string GetUsername() => Username;

    public string GetPassword() => Password;
    public string GetNickname() => Nickname;
    public string GetUserGroup() => UserGroup;
    public string GetModes() => Modes;
    public EnumUserAccessLevel GetLevel() => Level;
}