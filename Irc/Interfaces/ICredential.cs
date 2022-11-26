using Irc.Enumerations;

namespace Irc.Extensions.Security;

public interface ICredential
{
    string GetDomain();
    string GetUsername();
    string GetPassword();
    string GetNickname();
    string GetUserGroup();
    string GetModes();
    long GetIssuedAt();
    EnumUserAccessLevel GetLevel();
}