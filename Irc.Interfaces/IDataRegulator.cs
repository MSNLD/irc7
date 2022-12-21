namespace Irc.Interfaces;

public interface IDataRegulator
{
    bool IsIncomingThresholdExceeded();
    bool IsOutgoingThresholdExceeded();
    int GeIncomingQueueLength();
    int GetOutgoingQueueLength();
    int GetIncomingBytes();
    int GetOutgoingBytes();
    int PushIncoming(IMessage message);
    int PushOutgoing(string message);
    IMessage PopIncoming();
    IMessage PeekIncoming();
    string PopOutgoing();
    void Purge();
}