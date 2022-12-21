using Irc.Models.Enumerations;

namespace Irc.Interfaces;

public interface ISupportPackage
{
    ISupportPackage CreateInstance(ICredentialProvider credentialProvider);
    string CreateSecurityChallenge();
    EnumSupportPackageSequence InitializeSecurityContext(string token, string ip);
    EnumSupportPackageSequence AcceptSecurityContext(string token, string ip);
    string GetDomain();
    string GetPackageName();
    ICredential GetCredentials();
    bool IsAuthenticated();
}