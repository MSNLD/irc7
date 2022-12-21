using Irc.Interfaces;

namespace Irc.Security;

public class SecurityManager : ISecurityManager
{
    private readonly Dictionary<string, ISupportPackage> _supportProviders =
        new(StringComparer.InvariantCultureIgnoreCase);

    private string _supportedPackages = string.Empty;

    public void AddSupportPackage(ISupportPackage supportPackage)
    {
        _supportProviders.Add(supportPackage.GetType().Name, supportPackage);
        UpdateSupportPackages();
    }

    public ISupportPackage CreatePackageInstance(string name, ICredentialProvider credentialProvider)
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
            _supportProviders.Where(provider => ((SupportPackage)provider.Value).Listed)
                .Select(provider => provider.Value.GetPackageName()).Reverse());
    }
}