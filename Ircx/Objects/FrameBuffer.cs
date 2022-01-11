using System.Collections.Generic;

namespace Core.Ircx.Objects;

public class FrameBuffer
{
    public Queue<Frame> Queue = new();
}