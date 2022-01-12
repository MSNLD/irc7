using System.Collections.Generic;

namespace Irc.Worker.Ircx.Objects;

public class FrameBuffer
{
    public Queue<Frame> Queue = new();
}