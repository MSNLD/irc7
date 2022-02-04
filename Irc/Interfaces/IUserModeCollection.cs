namespace Irc.Worker;

public interface IUserModeCollection
{
    IMode GetMode(string mode);
    IMode ResolveMode(byte ModeChar);
    void UpdateModes();
    string ToString();
}