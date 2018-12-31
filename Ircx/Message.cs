using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;

namespace Core.Ircx
{
    public class Message
    {
        public static short whiteSpace = 0x20;

        public String8 rawData, Prefix, Command;
        public List<String8> Data;
        // Temp stuff
        public int ParamOffset;
        public String8 GetNextParam()
        {
            if (ParamOffset < Data.Count) { return Data[ParamOffset++]; }
            else { return null; }
        }
        // Temp stuff
        public Message(String8 incomingData)
        {
            rawData = incomingData;
            Parse();
        }

        private void Parse()
        {
            if (rawData == null) return;
            if (rawData.Length == 0) return;

            int offset = 0;

            for (; ((offset < rawData.Length) && (rawData.bytes[offset] == whiteSpace)); offset++) ; //trimming

            if (rawData.bytes[offset] == ':') {
                Prefix = GetWord(++offset);
                //if (Prefix == null) return;
                if (Prefix != null) { 
                    offset += Prefix.Length + 1;
                }
            }

            while ((Command == null) && (offset < rawData.Length)) { Command = GetWord(offset); offset++; }

            if (Command == null) return;
            Command.toupper();
            

            offset += Command.Length;

            Data = GetParams(offset);
        }

        private String8 GetWord(int offset)
        {
            int c;
            for (c = offset; c < rawData.Length; c++)
            {
                if (rawData.bytes[c] == whiteSpace)
                {
                    if ((c - offset) <= 1) { return null; }
                    else { return new String8(rawData.bytes, offset, c); }
                }
            }

            if (c == rawData.Length) { return new String8(rawData.bytes, offset, c); }
            else { return null; }
        }
        private List<String8> GetParams(int offset)
        {
            if ((rawData.bytes.Length - offset) <= 0) return null;

            List<String8> Data = new List<String8>();

            if ((rawData.bytes[offset] == 58) && (offset + 1 < rawData.bytes.Length)) { Data.Add(new String8(rawData.bytes, offset + 1, rawData.bytes.Length)); return Data; }
            else if ((rawData.bytes[offset] == 58) && (offset + 1 == rawData.bytes.Length)) { Data.Add(Resources.Null); }
            else { 
                for (int i = offset; i < rawData.bytes.Length; i++)
                {
                    if (rawData.bytes[i] == whiteSpace)
                    {
                        if ((i - offset) > 0) { Data.Add(new String8(rawData.bytes, offset, i)); } //add current parameter

                        if ((i + 1) < rawData.bytes.Length)
                        {
                            if (rawData.bytes[i + 1] == 0x3A) //check if next parameter has a :
                            {
                                //if (i + 2 == rawData.bytes.Length) { Data.Add(Resources.Null); } // Fix for blank messages

                                offset = i + 2; // if so break and catch the rest and save as param
                                break;
                            }
                        }

                        offset = i + 1;
                    }
                }
                Data.Add(new String8(rawData.bytes, offset, rawData.bytes.Length));
            }
            return Data;
        }
    }
}
