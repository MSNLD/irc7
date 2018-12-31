using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTools
{
    class StringBuffer : String8
    {
        //needs to be converted to a Queue later
        public List<String8> DataIn;
        public List<String8> DataOut;
        public List<ArraySegment<byte>> bytesOut;
        int buffSize, cursor;

        //e.g.                  1024      512

        public StringBuffer(int size)
            : base(size)
        {
            buffSize = size;
            cursor = 0;

            DataIn = new List<String8>();
            DataOut = new List<String8>();
            bytesOut = new List<ArraySegment<byte>>();
        }
        public void Digest(byte[] data, int bytes)
        {

            for (int dataPos = 0; dataPos < bytes; dataPos++)
            {
                if (cursor < buffSize)  //ensure the current cursor is < 512 bytes
                {
                    if ((data[dataPos] != '\r') && (data[dataPos] != '\n'))
                    {
                        base.bytString[cursor++] = data[dataPos]; //copy in
                    }
                    else
                    {
                        if (cursor > 0) //to stop problems with joined crlf
                        {
                            String8 message = new String8(base.bytString, 0, cursor);
                            cursor = 0;
                            DataIn.Add(message);
                        }
                    }
                }
                else
                {
                    String8 message = new String8(base.bytString, 0, cursor);
                    cursor = 0;
                    DataIn.Add(message);
                }
            }

            if (cursor == buffSize)
            {
                String8 message = new String8(base.bytString, 0, cursor);
                cursor = 0;
                DataIn.Add(message);
            }

        }
    };
}
