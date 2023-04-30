using Irc.Enumerations;
using Irc.Objects;

namespace Irc.Extensions.Interfaces
{
    public interface IPropRule
    {
        EnumChannelAccessLevel ReadAccessLevel { get; }
        EnumChannelAccessLevel WriteLevel { get; }
        string Name { get; }
        bool ReadOnly { get; }
        string GetValue();
        EnumIrcError SetValue(string value, ChatObject source = null);
    }
}