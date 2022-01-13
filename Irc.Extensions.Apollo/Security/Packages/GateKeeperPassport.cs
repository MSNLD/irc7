using System.Globalization;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Apollo.Security.Credentials;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;

namespace Irc.Extensions.Apollo.Security.Packages;

public class GateKeeperPassport : GateKeeper
{
    private readonly ICredentialProvider _credentialProvider;
    public string Puid;

    public GateKeeperPassport(ICredentialProvider credentialProvider)
    {
        _credentialProvider = credentialProvider;
        ServerSequence = EnumSupportPackageSequence.SSP_INIT;
        Guest = false;
        Listed = false;
    }

    public override string GetDomain()
    {
        return nameof(GateKeeperPassport);
    }

    public new SupportPackage CreateInstance(ICredentialProvider credentialProvider)
    {
        return new GateKeeperPassport(credentialProvider);
    }

    public override EnumSupportPackageSequence AcceptSecurityContext(string data, string ip)
    {
        if (ServerSequence != EnumSupportPackageSequence.SSP_CREDENTIALS)
        {
            var s = base.AcceptSecurityContext(data, ip);
            if (s == EnumSupportPackageSequence.SSP_OK)
            {
                Authenticated = false;
                ServerSequence = EnumSupportPackageSequence.SSP_CREDENTIALS;
                return EnumSupportPackageSequence.SSP_CREDENTIALS;
            }

            return s;
        }

        if (data.Length >= 8)
        {
            int _tLen, _pLen;
            bool tConvSuccess;

            tConvSuccess = int.TryParse(StringBuilderExtensions.FromBytes(data.ToByteArray(), 0, 8).ToString(),
                NumberStyles.HexNumber, null, out _tLen);
            if (tConvSuccess)
                if (data.Length >= 16 + _tLen)
                {
                    tConvSuccess =
                        int.TryParse(
                            StringBuilderExtensions.FromBytes(data.ToByteArray(), _tLen + 8, _tLen + 16).ToString(),
                            NumberStyles.HexNumber, null, out _pLen);
                    if (tConvSuccess)
                        if (_tLen > 0 && _pLen > 0 && data.Length >= 16 + _tLen + _pLen)
                        {
                            StringBuilder ticket, profile;

                            ticket = StringBuilderExtensions.FromBytes(data.ToByteArray(), 8, 8 + _tLen);
                            profile = StringBuilderExtensions.FromBytes(data.ToByteArray(), _tLen + 16,
                                _tLen + 16 + _pLen);

                            var t = _credentialProvider.Decrypt(ticket);
                            if (t == null) return EnumSupportPackageSequence.SSP_FAILED;

                            var p = _credentialProvider.Decrypt(profile, t.iv);
                            if (p == null) return EnumSupportPackageSequence.SSP_FAILED;

                            var memberIdLow = ulong.Parse(t.puid, NumberStyles.HexNumber);

                            if (memberIdLow != 0)
                            {
                                Guid = new Guid(t.puid);
                                Puid = new StringBuilder(p.origId).ToString();
                                ServerSequence = EnumSupportPackageSequence.SSP_AUTHENTICATED;
                                Authenticated = true;
                                return EnumSupportPackageSequence.SSP_OK;
                            }
                        }
                }
        }

        return EnumSupportPackageSequence.SSP_FAILED;
    }
}