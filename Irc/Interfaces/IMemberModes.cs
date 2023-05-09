namespace Irc.Interfaces;

public interface IMemberModes
{
    string GetModeString();
    string GetListedMode();

    char GetModeChar();

    bool IsOwner();
    bool IsHost();
    bool IsVoice();

    bool IsNormal();

    void SetOwner(bool flag);
    void SetHost(bool flag);
    void SetVoice(bool flag);
    void SetNormal();
}