using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Security.Packages;

public class ANON : SupportPackage
{
    public ANON()
    {
        Guest = true;
        Authenticated = true;
        Listed = false;
    }

    public override EnumSupportPackageSequence InitializeSecurityContext(string data, string ip)
    {
        return EnumSupportPackageSequence.SSP_AUTHENTICATED;
    }

    public override EnumSupportPackageSequence AcceptSecurityContext(string data, string ip)
    {
        return EnumSupportPackageSequence.SSP_AUTHENTICATED;
    }

    public override string GetDomain()
    {
        return nameof(ANON);
    }

    public override string GetPackageName()
    {
        return nameof(ANON);
    }

    public override SupportPackage CreateInstance(ICredentialProvider credentialProvider)
    {
        return new ANON();
    }

    public string CreateSecurityChallenge(EnumSupportPackageSequence stage)
    {
        return string.Empty;
    }
}