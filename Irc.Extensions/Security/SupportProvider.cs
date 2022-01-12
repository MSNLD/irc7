using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Apollo.Security.Credentials;

namespace Irc.Extensions.Security
{
    public class SupportProvider
    {
        public string SupportedPackages { get; private set; } = string.Empty;

        private readonly Dictionary<string, SupportPackage> _supportProviders = new(StringComparer.InvariantCultureIgnoreCase);

        public void AddSupportProvider(SupportPackage supportPackage)
        {
            _supportProviders.Add(supportPackage.GetDomain().ToUpper(), supportPackage);
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

        private void UpdateSupportPackages()
        {
            SupportedPackages = string.Join(',', _supportProviders.Where(provider => provider.Value.Listed).Select(provider => provider.Value.GetDomain()));
        }
    }
}
