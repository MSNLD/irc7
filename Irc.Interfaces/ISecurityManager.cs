namespace Irc.Interfaces;

public interface ISecurityManager
{
    void AddSupportPackage(ISupportPackage supportPackage);
    ISupportPackage CreatePackageInstance(string name, ICredentialProvider credentialProvider);
    string GetSupportedPackages();
}