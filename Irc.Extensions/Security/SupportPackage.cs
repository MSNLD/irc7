using Irc.Extensions.Apollo.Security.Credentials;

namespace Irc.Extensions.Security;
// Lazy implementation of my own idea of SupportPackage

public abstract partial class SupportPackage
{
    public Guid Guid;
    public bool Guest;
    public bool Authenticated { get; protected set; }
    public bool Listed = true;

    public uint ServerVersion;
    public EnumSupportPackageSequence ServerSequence;
    public Credentials UserCredentials;

    public abstract SupportPackage CreateInstance(ICredentialProvider credentialProvider);
    public abstract string CreateSecurityChallenge(EnumSupportPackageSequence stage);
    public abstract EnumSupportPackageSequence InitializeSecurityContext(string data, string ip);
    public abstract EnumSupportPackageSequence AcceptSecurityContext(string data, string ip);
    public abstract string GetDomain();
}