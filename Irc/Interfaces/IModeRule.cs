using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Interfaces
{
    public interface IModeRule
    {
        void Set(int value);
        int Get();
        char GetModeChar();
        bool RequiresParameter { get; }

        EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter);
    }
}