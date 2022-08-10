using Irc.Interfaces;

namespace Irc.Objects;

public class ModeCollection : IModeCollection
{
    protected Dictionary<char, IModeRule> modes = new();
    // TODO: <CHANKEY> Below is temporary until implemented properly
    protected string keypass = "test_key";

    public void SetModeChar(char mode, int value)
    {
        if (modes.ContainsKey(mode)) modes[mode].Set(value);
    }

    public int GetModeChar(char mode)
    {
        modes.TryGetValue(mode, out var value);
        return value.Get();
    }
    public IModeRule GetMode(char mode)
    {
        modes.TryGetValue(mode, out var value);
        return value;
    }

    public string GetSupportedModes()
    {
        return new(modes.Keys.OrderBy(x => x).ToArray());
    }

    public bool HasMode(char mode)
    {
        return modes.Keys.Contains(mode);
    }

    public override string ToString()
    {
        return $"+{new string(modes.Where(mode => mode.Value.Get() > 0).Select(mode => mode.Key).ToArray())}";
    }
}