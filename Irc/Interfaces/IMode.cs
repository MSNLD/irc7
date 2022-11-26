namespace Irc.Interfaces;

public interface IMode
{
    uint GetLevel();
    void SetLevel(uint level);
    byte GetModeChar();
    void SetModeChar(byte modeChar);
    int GetValue();
    void SetValue(int value);
}