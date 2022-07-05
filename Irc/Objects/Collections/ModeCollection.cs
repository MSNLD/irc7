namespace Irc.Objects;

public class ModeCollection : IModeCollection
{
    protected Dictionary<char, int> modes = new();
    // TODO: <CHANKEY> Below is temporary until implemented properly
    private string keypass = "test_key";

    public void SetModeChar(char mode, int value)
    {
        if (modes.ContainsKey(mode)) modes[mode] = value;
    }

    public int GetModeChar(char mode)
    {
        modes.TryGetValue(mode, out var value);
        return value;
    }

    public string GetSupportedModes()
    {
        return new(modes.Keys.OrderBy(x => x).ToArray());
    }

    public override string ToString()
    {
        // TODO: <MODESTRING> Fix the below for Limit and Key on mode string
        var limit = modes['l'] > 0 ? $" {modes['l']}" : string.Empty;
        var key = modes['k'] != 0 ? $" {keypass}" : string.Empty;

        return $"{new string(modes.Where(mode => mode.Value > 0).Select(mode => mode.Key).ToArray())}{limit}{key}";
    }
}