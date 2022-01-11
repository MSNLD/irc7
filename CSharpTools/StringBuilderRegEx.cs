using System.Text;
using System.Text.RegularExpressions;
using Core.CSharpTools;

namespace CSharpTools;

static class StringBuilderRegEx
{
    static string sRegExEval = new string('\0', 512);
    static StringBuilder sbRegExQuery = new StringBuilder(512);
    static StringBuilder sbRegExEval = new StringBuilder(512);

    public static bool EvaluateString(string query, string data, bool IgnoreCase)
    {
        sbRegExEval.Length = 0;
        sbRegExQuery.Length = 0;

        for (int i = 0; i < data.Length; i++)
        {
            sbRegExEval.Append((char)data.ToByteArray()[i]);
        }
        for (int i = 0; i < query.Length; i++)
        {
            if (query.ToByteArray()[i] == (byte)'*')
            {
                sbRegExQuery.Append((char)46); // .
                sbRegExQuery.Append((char)42); // *
            }
            else if (query.ToByteArray()[i] == (byte)'?')
            {
                sbRegExQuery.Append((char)46); // .
            }
            else
            {
                //escape all characters to avoid screwing up regular expressions
                sbRegExQuery.Append('\\');
                sbRegExQuery.Append('x');
                byte b;
                b = ((byte)(query.ToByteArray()[i] >> 4));
                sbRegExQuery.Append((char)(b > 9 ? b + 0x37 : b + 0x30));
                b = ((byte)(query.ToByteArray()[i] & 0xF));
                sbRegExQuery.Append((char)(b > 9 ? b + 0x37 : b + 0x30));
            }
        }
        Regex r = new Regex(sbRegExQuery.ToString(), (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
        return r.Match(sbRegExEval.ToString()).Success;
    }

    public static bool EvaluteEx(string sRegularExpression, string data, bool IgnoreCase, int offset, int length)
    {
        Regex r = new Regex(sRegularExpression, (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
        return r.Match(data.ToString(), offset, length).Success;
    }
    public static bool EvaluateAbs(string sRegularExpression, string data, bool IgnoreCase, int offset, int length)
    {
        Regex r = new Regex(sRegularExpression, (IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
        return (r.Match(data.ToString(), offset, length).Length == length);
    }
    public static bool Evalute(string sRegularExpression, string data, bool IgnoreCase) { return EvaluteEx(sRegularExpression, data, IgnoreCase, 0, data.Length); }
};