using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;

namespace Core.Authentication
{
    class ANON: SSP
    {
        public const UInt64 SIGNATURE = 0x1; //S2 0x0000005053534b47 ulong
        public override UInt64 Signature { get { return SIGNATURE; } }
        public new string NicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";
        public static string IRCOpNickMask = @"[\x41-\xFF\-0-9]+$";
        public override state InitializeSecurityContext(String8 data, String8 ip)
        {
            return state.SSP_AUTHENTICATED;
        }
        public override state AcceptSecurityContext(String8 data, String8 ip)
        {
            return state.SSP_AUTHENTICATED;
        }
        public ANON()
        {
            base.guest = true;
            base.IsAuthenticated = true;
        }
        public override SSP Create() { return new ANON(); }
        public override String8 GetDomain() { return null; }
        public override string GetNickMask() { return NicknameMask; }
        public override String8 CreateSecurityChallenge(state stage)
        {
             return null;
        }
    }
}
