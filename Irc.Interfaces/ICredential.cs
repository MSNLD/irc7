using Irc.Models.Enumerations;

namespace Irc.Interfaces;

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