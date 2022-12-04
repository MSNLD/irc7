﻿using System.Text;

namespace Irc.ClassExtensions.CSharpTools;

public static class Tools
{
    public static int Str2Int(string Data) //all negative mean failure
    {
        var result = 0;
        for (var i = 0; i < Data.Length; i++)
            if (Data[i] >= 48 && Data[i] <= 57)
                result += (Data[i] - 48) * (int)Math.Pow(10, Data.Length - i - 1);
            else
                return -1;
        return result;
    }

    public static int Str2Int(string Data, int iStartPos)
    {
        if (iStartPos >= Data.Length) return -1;

        var nString = new string(Data.Substring(iStartPos, Data.Length));
        return Str2Int(nString);
    }

    public static bool StringArrayContains(List<string> StringArray, string Data)
    {
        for (var i = 0; i < StringArray.Count; i++)
            if (StringArray[i] == Data)
                return true;
        return false;
    }

    public static List<string> CSVToArray(string CSV, bool bIgnoreBlanks, int maxFieldLen = 512)
    {
        var Values = new List<string>();
        var Value = new StringBuilder(maxFieldLen);
        for (var i = 0; i < CSV.Length; i++)
            if (CSV[i] != 44)
            {
                Value.Append(CSV[i]);
            }
            else
            {
                if (Value.Length > 0)
                {
                    var Field = new string(Value.ToString());
                    if (!StringArrayContains(Values, Field))
                    {
                        Values.Add(Field);
                        Value.Length = 0;
                    }
                }
                //else { return null; }
            }

        if (Value.Length > 0) Values.Add(new string(Value.ToString()));
        //else { if (!bIgnoreBlanks) { return null; } }

        return Values;
    }

    public static List<string> CSVToArray(string CSV)
    {
        return CSVToArray(CSV, false);
    }

    public static string HexToString(string Hex)
    {
        if (Hex.Length == 0) return null;

        var OutputString = new StringBuilder(Hex.Length / 2);
        var HexStr = Hex;

        for (var c = 0; c < OutputString.Length; c++)
            OutputString[c] = (char)Convert.ToByte(HexStr.Substring(c * 2, 2));

        return OutputString.ToString();
    }
}