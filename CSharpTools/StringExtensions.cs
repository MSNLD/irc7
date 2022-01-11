using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpTools;

namespace Core.CSharpTools
{
    public static class StringExtensions
    {
        public static byte[] ToByteArray(this String text, int offset, int count)
        {
            byte[] b = new byte[count];
            for (var i = 0; i < count; i++)
            {
                b[i] = (byte)text[i];
            }
            return b;
        }
        public static byte[] ToByteArray(this String text)
        {
            return text.ToByteArray(0, text.Length);
        }
        public static byte[] GetBytes(this StringBuilder text)
        {
            byte[] b = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                b[i] = (byte)text[i];
            }

            return b;
        }


        public static string ToLiteral(string data)
        {
            StringBuilder literal = new StringBuilder(data.Length);
            
            for (int i = 0, c = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case '\\':
                    {
                        i++;
                        if (i < data.Length)
                        {
                            switch (data[i])
                            {
                                case 'b':
                                {
                                    literal.Append(' ');
                                    break;
                                }
                                case '0':
                                {
                                    literal.Append('\0');
                                    ;
                                    break;
                                }
                                case 't':
                                {
                                    literal.Append('\t');
                                    break;
                                }
                                case 'c':
                                {
                                    literal.Append(',');
                                    break;
                                }
                                case '\\':
                                {
                                    literal.Append('\\');
                                    break;
                                }
                                case 'r':
                                {
                                    literal.Append('\r');
                                    break;
                                }
                                case 'n':
                                {
                                    literal.Append('\n');
                                    break;
                                }
                                default:
                                {
                                    literal.Append((char)data[i - 1]);
                                    literal.Append((char)data[i]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            literal.Append((char)data[i - 1]);
                        }

                        break;
                    }
                    default:
                    {
                        literal.Append((char)data[i]);
                        break;
                    }
                }
            }

            return literal.ToString();
        }

        public static string ToEscape(string data)
        {
            StringBuilder escape = new StringBuilder(data.Length * 2);

            for (int i = 0; i < data.Length; i++)
            {
                switch (data[i])
                {
                    case (char)0x0:
                    {
                        escape.Append('\\');
                        escape.Append('0');
                        break;
                    }
                    case (char)0x9:
                    {
                        escape.Append('\\');
                        escape.Append('t');
                        break;
                    }
                    case (char)0xD:
                    {
                        escape.Append('\\');
                        escape.Append('r');
                        break;
                    }
                    case (char)0xA:
                    {
                        escape.Append('\\');
                        escape.Append('n');
                        break;
                    }
                    case (char)0x20:
                    {
                        escape.Append('\\');
                        escape.Append('b');
                        break;
                    }
                    case (char)0x2C:
                    {
                        escape.Append('\\');
                        escape.Append('c');
                        break;
                    }
                    case (char)0x5C:
                    {
                        escape.Append('\\');
                        escape.Append('\\');
                        break;
                    }
                    default:
                    {
                        escape.Append((char)data[i]);
                        break;
                    }
                }
            }

            return escape.ToString();
        }

        public static StringBuilder FromBytes(byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder(bytes.Length);
            for (var i = 0; i < bytes.Length; i++)
            {
                stringBuilder.Append((char)bytes[i]);
            }
            return stringBuilder;
        }

        public static StringBuilder FromBytes(byte[] bytes, int start, int count)
        {
            StringBuilder stringBuilder = new StringBuilder(count);
            for (var i = start; i < count; i++)
            {
                stringBuilder.Append((char)bytes[i]);
            }
            return stringBuilder;
        }
        public static char[] BytesToChars(byte[] bytes, int offset, int count)
        {
            char[] c = new char[count];
            for (var i = 0; i < count; i++)
            {
                c[i] = (char)bytes[offset + i];
            }
            return c;
        }
        public static char[] BytesToChars(byte[] bytes)
        {
            return BytesToChars(bytes, 0, bytes.Length);
        }
    }
}
