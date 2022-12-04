using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Props;

public class PropRule : IPropRule
{
    public PropRule(string name, EnumChannelAccessLevel readAccessLevel, EnumChannelAccessLevel writeAccessLevel,
        string initialValue, bool readOnly = false)
    {
        Name = name;
        ReadAccessLevel = readAccessLevel;
        WriteLevel = writeAccessLevel;
        _value = initialValue;
        ReadOnly = readOnly;
    }

    private string _value { get; set; }

    public string Name { get; }
    public EnumChannelAccessLevel ReadAccessLevel { get; }
    public EnumChannelAccessLevel WriteLevel { get; }
    public bool ReadOnly { get; }

    public virtual void SetValue(string value)
    {
        _value = value;
    }

    public virtual string GetValue()
    {
        return _value;
    }

    public EnumIrcError SetValue(ChatObject source, string value)
    {
        throw new NotImplementedException();
    }
}