using Irc.Security;

namespace Irc.Extensions.Security;

public interface ISecurityManager
{
    void AddSupportPackage(SupportPackage supportPackage);
    SupportPackage CreatePackageInstance(string name, ICredentialProvider credentialProvider);
    string GetSupportedPackages();
}