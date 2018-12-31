using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Ircx.Objects
{
    public static class ObjIDGenerator
    {
        private static long maxOID = 0xFFFFFF;

        public static long ServerID { set; get; } // max 0xFF

        public static long OIDindex = 1;
        public static bool bSecondRun = true;

        public static HashSet<long> hsOID = new HashSet<long>();

        public static long New()
        {
            if (!bSecondRun)
            {
                OIDindex++;
                hsOID.Add(OIDindex);
                return OIDindex;
            }
            else
            {
                // Check from current OID to Max OID
                for (long c = OIDindex; c <= maxOID; c++) { 
                    if (!hsOID.Contains(c))
                    {
                        OIDindex = c;
                        return setOID(OIDindex);
                    }
                }
                // Check from 1 to current OID
                for (long c = 1; c < OIDindex; c++)
                {
                    if (!hsOID.Contains(c))
                    {
                        OIDindex = c;
                        return setOID(OIDindex);
                    }
                }
                // Return 0 aka full
                return 0;
            }
        }

        private static long setOID(long index)
        {
            hsOID.Add(index);
            return (long)((long)ServerID << 24) + index;
        }

        public static void Free(long OID)
        {
            OID = (0x00FFFFFF & OID);
            hsOID.Remove(OID);
        }
    }
}
