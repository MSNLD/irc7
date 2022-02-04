namespace Irc.Worker;

public interface IMode
{
    uint GetLevel();
    void SetLevel(uint level);
    byte GetModeChar();
    void SetModeChar(byte modeChar);
    int GetValue();
    void SetValue(int value);
}