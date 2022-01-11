namespace Core.Authentication;

internal class ANON : SSP
{
    public const ulong SIGNATURE = 0x1; //S2 0x0000005053534b47 ulong
    public static string IRCOpNickMask = @"[\x41-\xFF\-0-9]+$";
    public new string NicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";

    public ANON()
    {
        guest = true;
        IsAuthenticated = true;
    }

    public override ulong Signature => SIGNATURE;

    public override state InitializeSecurityContext(string data, string ip)
    {
        return state.SSP_AUTHENTICATED;
    }

    public override state AcceptSecurityContext(string data, string ip)
    {
        return state.SSP_AUTHENTICATED;
    }

    public override SSP Create()
    {
        return new ANON();
    }

    public override string GetDomain()
    {
        return null;
    }

    public override string GetNickMask()
    {
        return NicknameMask;
    }

    public override string CreateSecurityChallenge(state stage)
    {
        return null;
    }
}