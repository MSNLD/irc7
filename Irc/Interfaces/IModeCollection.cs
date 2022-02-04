namespace Irc.Objects;

public interface IModeCollection
{
    void SetModeChar(char mode, int value);
    int GetModeChar(char mode);
    string GetSupportedModes();
    string ToString();
}