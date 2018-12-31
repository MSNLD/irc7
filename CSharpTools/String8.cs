using System;
//using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

#pragma warning disable 660, 661

namespace CSharpTools
{

    // The idea was to have a class that represented 8bit strings
    // as lots of problems in the past using .NET string class

    static class String8RegEx
    {
        static string sRegExEval = new string('\0', 512);
        static StringBuilder sbRegExQuery = new StringBuilder(512);
        static StringBuilder sbRegExEval = new StringBuilder(512);

        public static bool EvaluateString8(String8 query, String8 data, bool IgnoreCase)
        {
            sbRegExEval.Length = 0;
            sbRegExQuery.Length = 0;

            for (int i = 0; i < data.length; i++)
            {
                sbRegExEval.Append((char)data.bytes[i]);
            }
            for (int i = 0; i < query.length; i++)
            {
                if (query.bytes[i] == (byte)'*')
                {
                    sbRegExQuery.Append((char)46); // .
                    sbRegExQuery.Append((char)42); // *
                }
                else if (query.bytes[i] == (byte)'?')
                {
                    sbRegExQuery.Append((char)46); // .
                }
                else
                {
                    //escape all characters to avoid screwing up regular expressions
                    sbRegExQuery.Append('\\');
                    sbRegExQuery.Append('x');
                    byte b;
                    b = ((byte)(query.bytes[i] >> 4));
                    sbRegExQuery.Append((char)(b > 9 ? b + 0x37 : b + 0x30));
                    b = ((byte)(query.bytes[i] & 0xF));
                    sbRegExQuery.Append((char)(b > 9 ? b + 0x37 : b + 0x30));
                }
            }
            Regex r = new Regex(sbRegExQuery.ToString(), (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
            return r.Match(sbRegExEval.ToString()).Success;
        }

        public static bool EvaluteEx(string sRegularExpression, String8 data, bool IgnoreCase, int offset, int length)
        {
            Regex r = new Regex(sRegularExpression, (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
            return r.Match(data.chars, offset, length).Success;
        }
        public static bool EvaluateAbs(string sRegularExpression, String8 data, bool IgnoreCase, int offset, int length)
        {
            Regex r = new Regex(sRegularExpression, (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
            return (r.Match(data.chars, offset, length).Length == length);
        }
        public static bool Evalute(string sRegularExpression, String8 data, bool IgnoreCase) { return EvaluteEx(sRegularExpression, data, IgnoreCase, 0, data.length); }
    };

    public class String8
    {
        //[DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        //static extern int memcmp(byte[] b1, byte[] b2, long count);
        
        public static int memcmp(byte[] b1, byte[] b2, long count)
        {
            //if (b1.Length != b2.Length) { return -1; }
            for (int c = 0; c < count; c++)
            {
                if (b1[c] != b2[c]) { return -1; }
            }
            return 0;
        }

        protected byte[] bytString;
        public int length;
        public int Length { get { return length; } }

        public int Capacity
        {
            get { return bytes.Length; }
        }

        public void assign(byte c, int pos)
        {
            bytString[pos] = c;
        }

        public static byte[] tobyte(string data, int offset, int length)
        {
            byte[] b = new byte[length];
            for (int i = offset, c = 0; i < offset + length; i++, c++)
            {
                b[c] = (byte)(data[i] & 0xFF);
            }
            return b;
        }
        public static byte[] tobyte(string data)
        {
            return tobyte(data, 0, data.Length);
        }
        public static byte[] toutf16(byte[] data)
        {
            bool PackUTF16 = true;
            int len = data.Length;
            byte[] utf16 = new byte[len * 2];
            for (int i = 0, c = 0; i < len; c++)
            {
                if (PackUTF16) { utf16[c] = data[i++]; PackUTF16 = false; }
                else { utf16[c] = 0; PackUTF16 = true; }
            }
            return utf16;
        }
        public static byte[] fromutf16(byte[] data)
        {
            int len = data.Length;
            byte[] utf8 = new byte[len / 2];
            for (int i = 0, c = 0; i < len; i++)
            {
                if (c < utf8.Length)
                {
                    if (data[i] != 0) { utf8[c++] = data[i]; }
                }
            }
            return utf8;
        }
        public void reverse()
        {
            byte[] b = new byte[bytString.Length];
            for (int i = 0; i < bytString.Length; i++)
            {
                b[bytString.Length - i - 1] = bytString[i];
            }
            bytString = b;
        }
        public void assign(string data)
        {
            if (data == null) { return; }
            bytString = tobyte(data);
            length = bytString.Length;
        }
        //public void concat(String8 c2)
        //{
        //    byte[] b = new byte[length + c2.length];
        //    for (int i = 0; i < length; i++) { b[i] = bytString[i]; }
        //    for (int i = length; i < b.Length; i++) { b[i] = c2.bytes[i - length]; }
        //}

        public int append(byte b)
        {
            if (length != Capacity)
            {
                bytString[length++] = b;
                return 1;
            }
            return 0;
        }
        public int append(String8 c2)
        {
            Array.Copy(c2.bytes, 0, bytString, length, c2.length);
            length += c2.length;
            return c2.length;

            //for (int i = 0; i < c2.length; i++) { append(c2.bytes[i]); }
            //return c2.length;
        }
        public int append(byte[] b)
        {
            Array.Copy(b, 0, bytString, length, b.Length);
            length += b.Length;
            return b.Length;
            //for (int i = 0; i < b.Length; i++) { append(b[i]); }
            //return b.Length;
        }
        public int append(byte[] b, int offset, int count)
        {
            Array.Copy(b, offset, bytString, length, count);
            length += count;
            return count;

            //for (int i = offset; i < count; i++) { append(b[i]); }
            //return count;
        }

        public int append(char c) { append((byte)c); return 1; }
        public string getChars()
        {
            StringBuilder s = new StringBuilder(bytString.Length);
            for (int i = 0; i < bytString.Length; i++)
            {
                s.Append((char)bytString[i]);
            }

            return s.ToString();
        }
        public void toupper()
        {
            for (int i = 0; i < bytString.Length; i++)
            {
                if ((bytString[i] >= 'a') && (bytString[i] <= 'z')) { bytString[i] -= 0x20; } // Convert to uppercase equivalent
            }
        }
        public void Salt()
        {
            Random r = new Random((int)DateTime.Now.Ticks);
            for (int i = length + 1; i < Capacity; i++)
            {
                bytString[i] = (byte)r.Next(0, 255);
            }
        }
        public static String8 ToLiteral(String8 data)
        {
            String8 literal = new String8(data.length);

            for (int i = 0, c = 0; i < data.length; i++)
            {
                switch (data.bytes[i])
                {
                    case (byte)'\\':
                        {
                            i++;
                            if (i < data.length)
                            {
                                switch (data.bytes[i])
                                {
                                    case (byte)'b': { literal.append(' '); break; }
                                    case (byte)'0': { literal.append('\0'); ; break; }
                                    case (byte)'t': { literal.append('\t'); break; }
                                    case (byte)'c': { literal.append(','); break; }
                                    case (byte)'\\': { literal.append('\\'); break; }
                                    case (byte)'r': { literal.append('\r'); break; }
                                    case (byte)'n': { literal.append('\n'); break; }
                                    default: { literal.append(data.bytes[i - 1]); literal.append(data.bytes[i]); break; }
                                }
                            }
                            else { literal.append(data.bytes[i - 1]); }

                            break;
                        }
                    default: { literal.append(data.bytes[i]); break; }
                }
            }
            return literal;
        }
        public static String8 ToEscape(String8 data)
        {
            String8 escape = new String8(data.length * 2);

            for (int i = 0; i < data.length; i++)
            {
                switch (data.bytes[i])
                {
                    case 0x0: { escape.append('\\'); escape.append('0'); break; }
                    case 0x9: { escape.append('\\'); escape.append('t'); break; }
                    case 0xD: { escape.append('\\'); escape.append('r'); break; }
                    case 0xA: { escape.append('\\'); escape.append('n'); break; }
                    case 0x20: { escape.append('\\'); escape.append('b'); break; }
                    case 0x2C: { escape.append('\\'); escape.append('c'); break; }
                    case 0x5C: { escape.append('\\'); escape.append('\\'); break; }
                    default: { escape.append(data.bytes[i]); break; }
                }
            }

            return escape;
        }

        public void Resize(int size)
        {
            byte[] b1 = bytString;
            bytString = new byte[size];
            Array.Copy(b1, bytString, length);
            length = size;
        }

        public void XorBytes(byte xorByte)
        {
            for (int c = 0; c < bytString.Length; c++)
            {
                bytString[c] ^= xorByte;
            }
        }

        public string chars
        {
            get
            {
                return getChars();
            }
            set
            {
                assign(value);
            }
        }
        public byte[] bytes
        {
            get { return bytString; }
        }

        public String8() { }
        public String8(string data)
        {
            assign(data);
        }

        public static implicit operator String8(string s)
        {
            return new String8(s);

        }

        public String8(byte[] data)
        {
            length = data.Length;
            bytString = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytString[i] = data[i];
            }
        }
        public String8(byte[] data, byte NullTerminatingChar)
        {
            length = data.Length;
            bytString = new byte[length];
            for (int i = 0; i < length; i++)
            {
                if (data[i] == NullTerminatingChar) { length = i; break; }
                else { bytString[i] = data[i]; }
            }
        }
        public String8(byte[] data, bool PackUTF16)
        {
            length = data.Length;
            bytString = new byte[length * 2];
            for (int i = 0, c = 0; i < length; c++)
            {
                if (PackUTF16) { bytString[c] = data[i++]; PackUTF16 = false; }
                else { bytString[c] = 0; PackUTF16 = true; }
            }
        }
        public String8(byte[] data, int from, int to)
        {
            length = to - from;
            bytString = new byte[length];
            Array.Copy(data, from, bytString, 0, length);
        }
        public String8(int size)
        {
            bytString = new byte[size];
            length = 0;
        }
        public static bool compare(String8 c1, String8 c2, int length)
        {
            if ((c1.length < length) || (c2.length < length)) { return true; }
            else { return !(memcmp(c1.bytString, c2.bytString, length) == 0); }
        }

        public static int upperCaseChar(byte b)
        {
            // Compiler getting confused with => operator, what the fuck?
            if ((97 <= b) && (b <= 122)) { return b - 0x20; }
            else { return b; }
        }
        public static bool compareCaseInsensitive(String8 c1, String8 c2)
        {
            if (c1.Length != c2.Length) { return true; }
            else
            {
                for (int c = 0; c < c1.Length; c++)
                {
                    if (upperCaseChar(c1.bytes[c]) != upperCaseChar(c2.bytes[c])) { return true; }
                }
            }
            return false;
        }
        public bool compare(String8 c2, int length) { return compare(this, c2, length); }
        public static bool operator ==(String8 c1, String8 c2)
        {
            if ((object.ReferenceEquals(c1, null) && (object.ReferenceEquals(c2, null)))) { return true; }
            else if ((object.ReferenceEquals(c1, null) || (object.ReferenceEquals(c2, null)))) { return false; }
            else if (object.ReferenceEquals(c1, c2)) { return true; }

            return ((c1.bytString.Length == c2.bytString.Length) && (memcmp(c1.bytString, c2.bytString, c1.bytString.Length) == 0));
        }
        public static bool operator !=(String8 c1, String8 c2)
        {
            if ((object.ReferenceEquals(c1, null) && (object.ReferenceEquals(c2, null)))) { return false; }
            if ((object.ReferenceEquals(c1, null) || (object.ReferenceEquals(c2, null)))) { return true; }
            return !((c1.bytString.Length == c2.bytString.Length) && (memcmp(c1.bytString, c2.bytString, c1.bytString.Length) == 0));
        }
        public static String8 operator +(String8 c1, String8 c2)
        {
            String8 s = new String8(c1.bytes);
            s.append(c1);
            return s;
        }

    };
}
