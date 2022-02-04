using System.Net;
using Irc.Extensions.Security;
using Irc.Interfaces;

namespace Irc.Security;

public class SupportPackage : ISupportPackage
{
    protected ICredential _credentials;
    public bool Guest;
    public bool Authenticated { get; protected set; }
    public bool Listed = true;

    public uint ServerVersion;
    public EnumSupportPackageSequence ServerSequence;

    public virtual SupportPackage CreateInstance(ICredentialProvider credentialProvider)
    {
        throw new NotImplementedException();
    }

    public virtual string CreateSecurityChallenge()
    {
        throw new NotImplementedException();
    }

    public virtual EnumSupportPackageSequence InitializeSecurityContext(string token, string ip)
    {
        throw new NotImplementedException();
    }

    public virtual EnumSupportPackageSequence AcceptSecurityContext(string token, string ip)
    {
        throw new NotImplementedException();
    }

    public string GetDomain() => GetPackageName();

    public string GetPackageName() => this.GetType().Name;

    public ICredential GetCredentials() => _credentials;

    public bool IsAuthenticated() => Authenticated;
}