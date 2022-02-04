using System.Runtime.InteropServices;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.NTLM;
using Irc.Helpers.CSharpTools;
using Irc.Security;

namespace Irc.Extensions.Security.Packages
{
    // NTLM Implementation by Sky
    // Created: Long time ago...
    // NTLM is required for the CAC to work

    public partial class NTLM : SupportPackage
    {
        private NtlmType1Message _message1;
        private NtlmType2Message _message2;
        private NtlmType3Message _message3;

        private NTLMShared.TargetInformation _targetInformation = new NTLMShared.TargetInformation();

        public NTLM(ICredentialProvider credentialProvider)
        {
            Listed = true;
        }
        
        public SupportPackage CreateInstance(ICredentialProvider credentialProvider)
        {
            return new NTLM(credentialProvider);
        }

        public EnumSupportPackageSequence InitializeSecurityContext(string data, string ip)
        {
            try
            {
                _message1 = new NtlmType1Message(data);

                bool isOEM = !_message1.EnumeratedFlags[NtlmFlag.NTLMSSP_NEGOTIATE_UNICODE];

                _targetInformation.DomainName = isOEM ? "DOMAIN" : "DOMAIN".ToUnicodeString();
                _targetInformation.ServerName = isOEM ? "TK2CHATCHATA01" : "TK2CHATCHATA01".ToUnicodeString();
                _targetInformation.DNSDomainName = isOEM ? "TK2CHATCHATA01.Microsoft.Com" : "TK2CHATCHATA01.Microsoft.Com".ToUnicodeString();
                _targetInformation.DNSServerName = isOEM ? "TK2CHATCHATA01.Microsoft.Com" : "TK2CHATCHATA01.Microsoft.Com".ToUnicodeString();

                return EnumSupportPackageSequence.SSP_OK;
            }
            catch (Exception)
            {
                return EnumSupportPackageSequence.SSP_FAILED;
            }
        }

        public string CreateSecurityChallenge(EnumSupportPackageSequence stage)
        {
            try
            {
                _message2 = new NtlmType2Message(_message1.Flags, _targetInformation.DomainName, _targetInformation);
                return _message2.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public EnumSupportPackageSequence AcceptSecurityContext(string data, string ip)
        {
            try
            {
                _message3 = new NtlmType3Message(data);
                if (_message3.VerifySecurityContext(_message2.Challenge.ToAsciiString(), "password"))
                {
                    Authenticated = true;
                    return EnumSupportPackageSequence.SSP_OK;
                }
                return EnumSupportPackageSequence.SSP_FAILED;
            }
            catch (Exception)
            {
                return EnumSupportPackageSequence.SSP_FAILED;
            }
        }

        public new string GetDomain()
        {
            return ServerDomain;
        }

        public new string GetPackageName()
        {
            return nameof(NTLM);
        }

        public string ServerDomain { get; set; } = "cg";
    }
}