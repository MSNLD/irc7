using Irc.Extensions.Security;

namespace Irc.Security;

public class SecurityManager : ISecurityManager
{
    private readonly Dictionary<string, SupportPackage> _supportProviders =
        new(StringComparer.InvariantCultureIgnoreCase);

    private string _supportedPackages = string.Empty;

    public void AddSupportPackage(SupportPackage supportPackage)
    {
        _supportProviders.Add(supportPackage.GetType().Name, supportPackage);
        UpdateSupportPackages();
    }

    public SupportPackage CreatePackageInstance(string name, ICredentialProvider credentialProvider)
    {
        try
        {
            return _supportProviders[name].CreateInstance(credentialProvider);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public string GetSupportedPackages()
    {
        return _supportedPackages;
    }

    private void UpdateSupportPackages()
    {
        _supportedPackages = string.Join(',',
            _supportProviders.Where(provider => provider.Value.Listed)
                .Select(provider => provider.Value.GetPackageName()));
    }
}