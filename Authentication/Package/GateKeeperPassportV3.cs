using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;
using Core.Ircx.Objects;

namespace Core.Authentication.Package
{
    class GateKeeperPassport: GateKeeper
    {
        public static String8 DOMAIN = new String8("GateKeeperPassport");
        public const UInt64 SIGNATURE = 0x0000005053534b48; //S2 0x0000005053534b47 ulong
        public new string NicknameMask = @"^(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF][\x41-\xFF\-0-9]*$";
        public override UInt64 Signature { get { return SIGNATURE; } }
        public Passport3 passport;
        public String8 puid;

        public GateKeeperPassport()
        {
            server_sequence = (int)state.SSP_INIT;
            guest = false;
        }
        public override String8 GetDomain() { return GateKeeperPassport.DOMAIN; }
        public override SSP Create() { return new GateKeeperPassport(); }
        public override string GetNickMask() { return NicknameMask; }
        public override state AcceptSecurityContext(String8 data, String8 ip)
        {
            if (server_sequence != (int)state.SSP_CREDENTIALS)
            {
                state s = base.AcceptSecurityContext(data, ip);
                if (s == state.SSP_OK) { IsAuthenticated = false; server_sequence = (int)state.SSP_CREDENTIALS; return state.SSP_CREDENTIALS; }
                else { return s; }
            }
            else
            {
                if (data.length >= 8)
                {
                    int _tLen, _pLen;
                    bool tConvSuccess;

                    tConvSuccess = Int32.TryParse((new String8(data.bytes, 0, 8)).chars, System.Globalization.NumberStyles.HexNumber, null, out _tLen);
                    if (tConvSuccess)
                    {
                        if (data.length >= 16 + _tLen)
                        {
                            tConvSuccess = Int32.TryParse((new String8(data.bytes, _tLen + 8, _tLen + 16)).chars, System.Globalization.NumberStyles.HexNumber, null, out _pLen);
                            if (tConvSuccess)
                            {
                                if (((_tLen > 0) && (_pLen > 0)) && (data.length >= 16 + _tLen + _pLen))
                                {
                                    passport = new Passport3();
                                    String8 ticket = null, profile = null;

                                    ticket = new String8(data.bytes, 8, 8 + _tLen);
                                    profile = new String8(data.bytes, _tLen + 16, _tLen + 16 + _pLen);

                                    PassportTicket t = Passport3.Decrypt(ticket);
                                    if (t == null) { return state.SSP_FAILED; }

                                    PassportProfile p = Passport3.Decrypt(profile, t.iv);
                                    if (p == null) { return state.SSP_FAILED; }

                                    memberIdLow = ulong.Parse(t.puid, System.Globalization.NumberStyles.HexNumber);

                                    if (memberIdLow != 0)
                                    {
                                        uuid = (new String8(t.puid)).bytes;
                                        puid = (new String8(p.origId));
                                        server_sequence = (int)state.SSP_AUTHENTICATED;
                                        IsAuthenticated = true;
                                        return state.SSP_OK;
                                    }
                                }
                            }
                        }
                    }
                }
                return state.SSP_FAILED;
            }
        }

    }
}
