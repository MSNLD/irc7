using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Ircx.Objects
{
    public class FrameBuffer
    {
        public Queue<Frame> Queue = new Queue<Frame>();
    }
}
