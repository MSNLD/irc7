using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Apollo.Security.Credentials;

namespace Irc.Extensions.Security.Packages
{
    public class NTLM: SupportPackage
    {
        public override SupportPackage CreateInstance(ICredentialProvider credentialProvider)
        {
            return new NTLM();
        }

        public override string CreateSecurityChallenge(EnumSupportPackageSequence stage)
        {
            throw new NotImplementedException();
        }

        public override EnumSupportPackageSequence InitializeSecurityContext(string data, string ip)
        {
            throw new NotImplementedException();
        }

        public override EnumSupportPackageSequence AcceptSecurityContext(string data, string ip)
        {
            throw new NotImplementedException();
        }

        public override string GetDomain()
        {
            return nameof(NTLM);
        }
    }
}
