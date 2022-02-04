namespace Irc.Worker;

public interface IChatMemberModes
{
    string GetModeString();
    uint GetUserMode();
    byte GetModeChar();
    bool IsAdmin();
    bool IsOwner();
    bool IsHost();
    bool IsVoice();
    bool IsNormal();
    void SetAdmin(bool flag);
    void SetOwner(bool flag);
    void SetHost(bool flag);
    void SetVoice(bool flag);
    void UpdateFlag();
    void SetNormal();
}