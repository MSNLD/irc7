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
        public new const ulong SIGNATURE = 0x1; //S2 0x0000005053534b47 ulong
        public static string IRCOpNickMask = @"[\x41-\xFF\-0-9]+$";
        public new string NicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";

        public ANON()
        {
            guest = true;
            IsAuthenticated = true;
        }

        public override ulong Signature => SIGNATURE;

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
