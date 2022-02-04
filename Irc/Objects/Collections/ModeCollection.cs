namespace Irc.Objects;

public class ModeCollection : IModeCollection
{
    protected Dictionary<char, int> modes = new();

    public void SetModeChar(char mode, int value)
    {
        if (modes.ContainsKey(mode)) modes[mode] = value;
    }

    public int GetModeChar(char mode)
    {
        modes.TryGetValue(mode, out var value);
        return value;
    }

    public string GetSupportedModes() => new (modes.Keys.OrderBy(x => x).ToArray());

    public override string ToString()
    {
        return $"{new string(modes.Where(mode => mode.Value > 0).Select(mode => mode.Key).ToArray())}";
    }
}