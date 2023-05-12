using Irc.Interfaces;

namespace Irc.Objects;

public interface IModeCollection
{
    void SetModeChar(char mode, int value);
    void ToggleModeChar(char mode, bool flag);
    int GetModeChar(char mode);
    string GetModeString();
    IModeRule GetMode(char mode);
    IModeRule this[char mode] { get; }
    bool HasMode(char mode);
    string GetSupportedModes();
    string ToString();
}