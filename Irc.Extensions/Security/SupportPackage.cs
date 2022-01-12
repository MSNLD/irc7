using Irc.Extensions.Apollo.Security.Credentials;

namespace Irc.Extensions.Security;
// Lazy implementation of my own idea of SupportPackage

public abstract partial class SupportPackage
{
    public bool guest;
    protected bool IsAuthenticated;
    public ulong memberIdLow, memberIdHigh;
    public bool Listed = true;
    public byte[] Uuid;
    public bool Authenticated => IsAuthenticated;


    public uint ServerVersion;
    public EnumSupportPackageSequence ServerSequence;
    public Credentials UserCredentials;
    
    public virtual ulong Signature => 0;

    public abstract SupportPackage CreateInstance(ICredentialProvider credentialProvider);
    public abstract string CreateSecurityChallenge(EnumSupportPackageSequence stage);
    public abstract EnumSupportPackageSequence InitializeSecurityContext(string data, string ip);
    public abstract EnumSupportPackageSequence AcceptSecurityContext(string data, string ip);

    public abstract string GetDomain();
}