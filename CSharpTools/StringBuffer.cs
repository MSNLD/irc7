using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpTools
{
    class StringBuffer
    {
        //needs to be converted to a Queue later
        public List<string> DataIn;
        public List<string> DataOut;
        public List<ArraySegment<byte>> bytesOut;
        int buffSize, cursor;
        private StringBuilder _buffer;
        public int Capacity => _buffer.Capacity;

        //e.g.                  1024      512

        public StringBuffer(int size)

        {
            _buffer = new StringBuilder(new string('\0', size));
            buffSize = size;
            cursor = 0;

            DataIn = new List<string>();
            DataOut = new List<string>();
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
                        _buffer[cursor++] = (char)data[dataPos]; //copy in
                    }
                    else
                    {
                        if (cursor > 0) //to stop problems with joined crlf
                        {
                            StringBuilder message = new StringBuilder(_buffer.ToString().Substring(0, cursor));
                            cursor = 0;
                            DataIn.Add(message.ToString());
                        }
                    }
                }
                else
                {
                    StringBuilder message = new StringBuilder(_buffer.ToString().Substring(0, cursor));
                    cursor = 0;
                    DataIn.Add(message.ToString());
                }
            }

            if (cursor == buffSize)
            {
                StringBuilder message = new StringBuilder(_buffer.ToString().Substring(0, cursor));
                cursor = 0;
                DataIn.Add(message.ToString());
            }

        }
    };
}
