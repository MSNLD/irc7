using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Helpers.CSharpTools
{
    public static class ByteExtensions
    {
        // Because ASCIIEncoding.ASCII.GetString(bytes); is unreliable and returns ????
        public static string ToAsciiString(this byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length);
            foreach (byte b in bytes) sb.Append((char) b);
            return sb.ToString();
        }
        public static string ToUnicodeString(this byte[] bytes)
        {
            byte[] unicodeBytes = Encoding.Convert(Encoding.ASCII, Encoding.Unicode, bytes);
            return new string(unicodeBytes.Select(c => (char)c).ToArray());
        }
    }
}
