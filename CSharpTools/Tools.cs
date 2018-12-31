using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;

namespace Core.CSharpTools
{
    static class Tools
    {
        public static int Str2Int(String8 Data) //all negative mean failure
        {
            int result = 0;
            for (int i = 0; i < Data.length; i++)
            {
                if ((Data.bytes[i] >= 48) && (Data.bytes[i] <= 57))
                {
                    result += ((Data.bytes[i] - 48) * ((int)System.Math.Pow(10, (Data.length - i - 1))));
                }
                else { return -1; }
            }
            return result;
        }
        public static int Str2Int(String8 Data, int iStartPos)
        {
            if (iStartPos >= Data.length) { return -1; }

            String8 nString = new String8(Data.bytes, iStartPos, Data.length);
            return Str2Int(nString);
        }
        public static bool StringArrayContains(List<String8> StringArray, String8 Data)
        {
            for (int i = 0; i < StringArray.Count; i++)
            {
                if (StringArray[i] == Data) { return true; }
            }
            return false;
        }
        public static List<String8> CSVToArray(String8 CSV, bool bIgnoreBlanks)
        {
            List<String8> Values = new List<String8>();
            String8 Value = new String8(Core.Ircx.Objects.Address.MaxFieldLen);
            for (int i = 0; i < CSV.length; i++)
            {
                if (CSV.bytes[i] != 44) { Value.append(CSV.bytes[i]); }
                else
                {
                    if (Value.length > 0)
                    {
                        String8 Field = new String8(Value.bytes, 0, Value.length);
                        if (!StringArrayContains(Values, Field))
                        {
                            Values.Add(Field);
                            Value.length = 0;
                        }
                    }
                    //else { return null; }
                }
            }

            if (Value.length > 0) { Values.Add(new String8(Value.bytes, 0, Value.length)); }
            //else { if (!bIgnoreBlanks) { return null; } }

            return Values;
        }
        public static List<String8> CSVToArray(String8 CSV)
        {
            return CSVToArray(CSV, false);
        }
        public static String8 HexToString(String8 Hex)
        {
            if (Hex.Length == 0) { return null; }

            String8 OutputString = new String8(Hex.Length / 2);
            string HexStr = Hex.chars;

            for (int c = 0; c < OutputString.Length; c++)
            {
                OutputString.bytes[c] = Convert.ToByte(HexStr.Substring(c * 2, 2));
            }

            return OutputString;
        }
    }
}
