using Irc.Enumerations;

namespace Irc.Security;

public interface ICredential
{
    string GetDomain();
    string GetUsername();
    string GetPassword();
    string GetNickname();
    string GetUserGroup();
    string GetModes();
    long GetIssuedAt();
    bool Guest { get; set; }
    EnumUserAccessLevel GetLevel();
}