using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc
{
    public class DataRegulator : IDataRegulator
    {
        private readonly int _incomingByteThreshold;
        private readonly int _outgoingByteThreshold;
        private bool _incomingThresholdExceeded;
        private bool _outgoingThresholdExceeded;
        private int _incomingBytes;
        private int _outgoingBytes;

        readonly Queue<Message> _incomingQueue = new();
        readonly Queue<string> _outgoingQueue = new();

        public DataRegulator(int incomingByteThreshold, int outgoingByteThreshold)
        {
            _incomingByteThreshold = incomingByteThreshold;
            _outgoingByteThreshold = outgoingByteThreshold;
        }

        public int GetIncomingBytes() => _incomingBytes;

        public int GetOutgoingBytes() => _outgoingBytes;

        public bool IsIncomingThresholdExceeded() => _incomingThresholdExceeded;
        public bool IsOutgoingThresholdExceeded() => _outgoingThresholdExceeded;

        public int GeIncomingQueueLength() => _outgoingQueue.Count;
        public int GetOutgoingQueueLength() => _outgoingQueue.Count;
        public int PushIncoming(Message message)
        {
            _incomingQueue.Enqueue(message);
            _incomingBytes += message.OriginalText.Length;
            if (_incomingBytes > _incomingByteThreshold) _incomingThresholdExceeded = true;
            return _incomingBytes;
        }

        public int PushOutgoing(string message)
        {
            _outgoingQueue.Enqueue(message);
            _outgoingBytes += message.Length;
            if (_outgoingBytes > _outgoingByteThreshold) _outgoingThresholdExceeded = true;
            return _outgoingBytes;
        }

        public Message PeekIncoming()
        {
            return _incomingQueue.Peek();
        }

        public Message PopIncoming()
        {
            var message = _incomingQueue.Dequeue();
            _incomingBytes -= message.OriginalText.Length;
            return message;
        }

        public string PopOutgoing()
        {
            string message = _outgoingQueue.Dequeue();
            _outgoingBytes -= message.Length;
            return message;
        }

        public void Purge()
        {
            _incomingQueue.Clear();
            _outgoingQueue.Clear();
        }
    }
}
