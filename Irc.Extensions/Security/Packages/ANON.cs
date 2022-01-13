using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Apollo.Security.Credentials;

namespace Irc.Extensions.Security.Packages
{
    public class ANON: SupportPackage
    {
        public ANON()
        {
            Guest = true;
            Authenticated = true;
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

        public override SupportPackage CreateInstance(ICredentialProvider credentialProvider)
        {
            return new ANON();
        }

        public override string CreateSecurityChallenge(EnumSupportPackageSequence stage)
        {
            return null;
        }
    }
}
