using System.Globalization;
using System.Text;
using Core.CSharpTools;
using Core.Ircx.Objects;

namespace Core.Authentication.Package;

internal class GateKeeperPassport : GateKeeper
{
    public const ulong SIGNATURE = 0x0000005053534b48; //S2 0x0000005053534b47 ulong
    public static string DOMAIN = "GateKeeperPassport";
    public new string NicknameMask = @"^(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF][\x41-\xFF\-0-9]*$";
    public Passport3 passport;
    public string puid;

    public GateKeeperPassport()
    {
        server_sequence = (int) state.SSP_INIT;
        guest = false;
    }

    public override ulong Signature => SIGNATURE;

    public override string GetDomain()
    {
        return DOMAIN;
    }

    public override SSP Create()
    {
        return new GateKeeperPassport();
    }

    public override string GetNickMask()
    {
        return NicknameMask;
    }

    public override state AcceptSecurityContext(string data, string ip)
    {
        if (server_sequence != (int) state.SSP_CREDENTIALS)
        {
            var s = base.AcceptSecurityContext(data, ip);
            if (s == state.SSP_OK)
            {
                IsAuthenticated = false;
                server_sequence = (int) state.SSP_CREDENTIALS;
                return state.SSP_CREDENTIALS;
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
                            passport = new Passport3();
                            StringBuilder ticket = null, profile = null;

                            ticket = StringBuilderExtensions.FromBytes(data.ToByteArray(), 8, 8 + _tLen);
                            profile = StringBuilderExtensions.FromBytes(data.ToByteArray(), _tLen + 16,
                                _tLen + 16 + _pLen);

                            var t = Passport3.Decrypt(ticket);
                            if (t == null) return state.SSP_FAILED;

                            var p = Passport3.Decrypt(profile, t.iv);
                            if (p == null) return state.SSP_FAILED;

                            memberIdLow = ulong.Parse(t.puid, NumberStyles.HexNumber);

                            if (memberIdLow != 0)
                            {
                                uuid = new StringBuilder(t.puid).ToByteArray();
                                puid = new StringBuilder(p.origId).ToString();
                                server_sequence = (int) state.SSP_AUTHENTICATED;
                                IsAuthenticated = true;
                                return state.SSP_OK;
                            }
                        }
                }
        }

        return state.SSP_FAILED;
    }
}