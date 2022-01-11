using Core.CSharpTools;
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
        public static string Base64DefaultTableMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
        public static string Base64PassportTableMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!*$";
        public static string Base64RegCookieTableMap = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789^*@";

        string B64Value;

        public Base64()
        {
        }
        public Base64(string data)
        {
            B64Value = data;
        }

        public static string Encode(string data, B64MapType mapType, byte offsetChar)
        {
            string Base64Table;
            switch (mapType)
            {
                case B64MapType.MSPassport: { Base64Table = Base64PassportTableMap; break; }
                case B64MapType.MSRegCookie: { Base64Table = Base64RegCookieTableMap; break; }
                default: { Base64Table = Base64DefaultTableMap; break; }
            }

            int bytes = 3, c = 0, r = 0, padding = 0;
            StringBuilder text;
            if (data.Length < 4) { text = new StringBuilder(4); padding = 4 - data.Length; }

            r = data.Length % bytes;

            if (r != 0) { text = new StringBuilder(data.Length + (bytes - r)); padding += r; }
            else { text = new StringBuilder(data.Length); }

            text.Append(data);

            //byte[] b64 = new byte[((int)(Math.Ceiling((double)text.length / 3))) * 4];
            //byte[] b64 = new byte[text.length + (int)Math.Ceiling((double)text.length / 3)]; //add a 4th char to the array
            byte[] b64 = new byte[(text.Capacity / 3) * 4 + 1];
            if (offsetChar != 0) { b64[c++] = offsetChar; }

            for (int i = 0; i < text.Length; i += 3)
            {
                b64[c++] += (byte)Base64Table[(text.ToByteArray()[i] >> 2)];
                b64[c++] += (byte)Base64Table[(((text.ToByteArray()[i] & 0x03) << 4) | (text.ToByteArray()[i + 1] >> 4))];
                b64[c++] += (byte)Base64Table[(((text.ToByteArray()[i + 1] & 0x0F) << 2) | (text.ToByteArray()[i + 2] >> 6))];
                b64[c++] += (byte)Base64Table[(text.ToByteArray()[i + 2] & 0x3F)];
            }
            if (padding == 1)
            {
                b64[b64.Length - 2] = (byte)Base64Table[64];
                b64[b64.Length - 1] = (byte)Base64Table[64];
            }
            else if (padding == 2) { b64[b64.Length - 1] = (byte)Base64Table[64]; }
            string x = Convert.ToBase64String(b64);
            return System.Text.Encoding.ASCII.GetString(b64);
        }
        public string Encode() { return Encode(B64Value, B64MapType.Default, 0); }
        public string Encode(string data) { B64Value = data; return Encode(B64Value, B64MapType.Default, 0); }

        public static string Decode(string data, B64MapType mapType, bool bSkipFirstChar)
        {
            string Base64Table;
            switch (mapType)
            {
                case B64MapType.MSPassport: { Base64Table = Base64PassportTableMap; break; }
                case B64MapType.MSRegCookie: { Base64Table = Base64RegCookieTableMap; break; }
                default: { Base64Table = Base64DefaultTableMap; break; }
            }

            byte[] Base64ByteMap = new byte[256];

            int padding = 0, c = 0;

            StringBuilder plainText = new StringBuilder(data);
            byte[] b64 = new byte[(data.Length / 4) * 3];

            if (data.Length > 0) { if (data[data.Length - 1] == Base64Table[64]) { padding++; plainText[data.Length - 1] = Base64Table[0]; } }
            if (data.Length > 1) { if (data[data.Length - 2] == Base64Table[64]) { padding++; plainText[data.Length - 2] = Base64Table[0]; } }

            int i = (bSkipFirstChar ? 1 : 0);

            for (; i < plainText.Length; i += 4)
            {
                if (i + 4 <= plainText.Length)
                {
                    plainText[i] = (char)Base64ByteMap[i];
                    plainText[i + 1] = (char)Base64ByteMap[i + 1];
                    plainText[i + 2] = (char)Base64ByteMap[i + 2];
                    plainText[i + 3] = (char)Base64ByteMap[i + 3];

                    b64[c++] += (byte)((plainText[i] << 2) + ((plainText[i + 1] & 0x30) >> 4));
                    b64[c++] += (byte)(((plainText[i + 1] & 0x0F) << 4) + ((plainText[i + 2] & 0x3C) >> 2));
                    b64[c++] += (byte)(((plainText[i + 2] & 0x03) << 6) + plainText[i + 3]);
                }
                //else { return null; }
            }

            return System.Text.Encoding.ASCII.GetString(b64, 0, b64.Length - padding);
        }
    }
}
