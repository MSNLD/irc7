using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTools
{
    // Base64 implementation with charsets for PassportCookie/RegCookie
    public class Base64
    {
        public enum B64MapType { Default, MSPassport, MSRegCookie };
        // A lookup array would be much faster than a conditional function for the mappings
        public static String8 Base64DefaultTableMap = new String8("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=");
        public static String8 Base64PassportTableMap = new String8("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!*$");
        public static String8 Base64RegCookieTableMap = new String8("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789^*@");

        String8 B64Value;

        public Base64()
        {
        }
        public Base64(String8 data)
        {
            B64Value = new String8(data.bytes, 0, data.length);
        }
        public static void GetByteMapFromString(byte[] map, byte[] b)
        {
            /* for some reason a static calling a static wont get a return -.- */
            for (int i = 0; i <= map.Length - 1; i++)
            {
                byte c = (byte)map[i];
                b[c] = (byte)i;
            }
        }
        public static String8 Encode(String8 data, B64MapType mapType, byte offsetChar)
        {
            byte[] Base64Table;
            switch (mapType)
            {
                case B64MapType.MSPassport: { Base64Table = Base64PassportTableMap.bytes; break; }
                case B64MapType.MSRegCookie: { Base64Table = Base64RegCookieTableMap.bytes; break; }
                default: { Base64Table = Base64DefaultTableMap.bytes; break; }
            }

            int bytes = 3, c = 0, r = 0, padding = 0;
            String8 text;
            if (data.length < 4) { text = new String8(4); padding = 4 - data.length; }

            r = data.length % bytes;

            if (r != 0) { text = new String8(data.length + (bytes - r)); padding += r; }
            else { text = new String8(data.length); }

            text.append(data.bytes, 0, data.length);

            //byte[] b64 = new byte[((int)(Math.Ceiling((double)text.length / 3))) * 4];
            //byte[] b64 = new byte[text.length + (int)Math.Ceiling((double)text.length / 3)]; //add a 4th char to the array
            byte[] b64 = new byte[(text.Capacity / 3) * 4 + 1];
            if (offsetChar != 0) { b64[c++] = offsetChar; }

            for (int i = 0; i < text.length; i += 3)
            {
                b64[c++] += Base64Table[(text.bytes[i] >> 2)];
                b64[c++] += Base64Table[(((text.bytes[i] & 0x03) << 4) | (text.bytes[i + 1] >> 4))];
                b64[c++] += Base64Table[(((text.bytes[i + 1] & 0x0F) << 2) | (text.bytes[i + 2] >> 6))];
                b64[c++] += Base64Table[(text.bytes[i + 2] & 0x3F)];
            }
            if (padding == 1)
            {
                b64[b64.Length - 2] = Base64Table[64];
                b64[b64.Length - 1] = Base64Table[64];
            }
            else if (padding == 2) { b64[b64.Length - 1] = Base64Table[64]; }
            string x = Convert.ToBase64String(b64);
            return new String8(b64);
        }
        public String8 Encode() { return Encode(B64Value, B64MapType.Default, 0); }
        public String8 Encode(String8 data) { B64Value = data; return Encode(B64Value, B64MapType.Default, 0); }

        public static String8 Decode(String8 data, B64MapType mapType, bool bSkipFirstChar)
        {
            byte[] Base64Table;
            switch (mapType)
            {
                case B64MapType.MSPassport: { Base64Table = Base64PassportTableMap.bytes; break; }
                case B64MapType.MSRegCookie: { Base64Table = Base64RegCookieTableMap.bytes; break; }
                default: { Base64Table = Base64DefaultTableMap.bytes; break; }
            }

            byte[] Base64ByteMap = new byte[256];
            GetByteMapFromString(Base64Table, Base64ByteMap);

            int padding = 0, c = 0;

            String8 plainText = new String8(data.bytes, 0, data.length);
            byte[] b64 = new byte[(data.length / 4) * 3];

            if (data.length > 0) { if (data.bytes[data.length - 1] == Base64Table[64]) { padding++; plainText.bytes[data.length - 1] = Base64Table[0]; } }
            if (data.length > 1) { if (data.bytes[data.length - 2] == Base64Table[64]) { padding++; plainText.bytes[data.length - 2] = Base64Table[0]; } }

            int i = (bSkipFirstChar ? 1 : 0);

            for (; i < plainText.length; i += 4)
            {
                if (i + 4 <= plainText.length)
                {
                    plainText.bytes[i] = Base64ByteMap[plainText.bytes[i]];
                    plainText.bytes[i + 1] = Base64ByteMap[plainText.bytes[i + 1]];
                    plainText.bytes[i + 2] = Base64ByteMap[plainText.bytes[i + 2]];
                    plainText.bytes[i + 3] = Base64ByteMap[plainText.bytes[i + 3]];

                    b64[c++] += (byte)((plainText.bytes[i] << 2) + ((plainText.bytes[i + 1] & 0x30) >> 4));
                    b64[c++] += (byte)(((plainText.bytes[i + 1] & 0x0F) << 4) + ((plainText.bytes[i + 2] & 0x3C) >> 2));
                    b64[c++] += (byte)(((plainText.bytes[i + 2] & 0x03) << 6) + plainText.bytes[i + 3]);
                }
                //else { return null; }
            }

            return new String8(b64, 0, b64.Length - padding);
        }
    }
}
