using System.Collections.Generic;

namespace Irc.Worker.Ircx.Objects;

public static class ObjIDGenerator
{
    private static readonly long maxOID = 0xFFFFFF;

    public static long OIDindex = 1;
    public static bool bSecondRun = true;

    public static HashSet<long> hsOID = new();

    public static long ServerID { set; get; } // max 0xFF

    public static long New()
    {
        if (!bSecondRun)
        {
            OIDindex++;
            hsOID.Add(OIDindex);
            return OIDindex;
        }

        // Check from current OID to Max OID
        for (var c = OIDindex; c <= maxOID; c++)
            if (!hsOID.Contains(c))
            {
                OIDindex = c;
                return setOID(OIDindex);
            }

        // Check from 1 to current OID
        for (long c = 1; c < OIDindex; c++)
            if (!hsOID.Contains(c))
            {
                OIDindex = c;
                return setOID(OIDindex);
            }

        // Return 0 aka full
        return 0;
    }

    private static long setOID(long index)
    {
        hsOID.Add(index);
        return (ServerID << 24) + index;
    }

    public static void Free(long OID)
    {
        OID = 0x00FFFFFF & OID;
        hsOID.Remove(OID);
    }
}