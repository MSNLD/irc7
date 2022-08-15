using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces
{
    public interface IModeRule
    {
        void Set(int value);
        void Set(bool value);
        int Get();
        char GetModeChar();
        bool RequiresParameter { get; }

        EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter);
        void DispatchModeChange(ChatObject source, ChatObject target, bool flag, string parameter = null);
    }
}