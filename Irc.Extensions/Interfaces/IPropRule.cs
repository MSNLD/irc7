using Irc.Enumerations;

namespace Irc.Extensions.Interfaces
{
    public interface IPropRule
    {
        EnumChannelAccessLevel ReadAccessLevel { get; }
        EnumChannelAccessLevel WriteLevel { get; }
        string Name { get; }
        bool ReadOnly { get; }
        string GetValue();
        void SetValue(string value);
    }
}