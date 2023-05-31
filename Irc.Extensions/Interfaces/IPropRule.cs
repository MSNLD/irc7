using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Interfaces
{
    public interface IPropRule
    {
        EnumChannelAccessLevel ReadAccessLevel { get; }
        EnumChannelAccessLevel WriteAccessLevel { get; }
        EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue);
        EnumIrcError EvaluateGet(IChatObject source, IChatObject target);
        string Name { get; }
        bool ReadOnly { get; }
        string GetValue(IChatObject target);
        void SetValue(string value);
    }
}