using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces;

public interface IModeRule
{
    bool RequiresParameter { get; }
    void Set(int value);
    void Set(bool value);
    int Get();
    char GetModeChar();

    EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter);
    void DispatchModeChange(ChatObject source, ChatObject target, bool flag, string parameter = null);
}