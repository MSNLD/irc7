namespace Irc7d;

public class DefaultChannel
{
    public string Name { get; set; }
    public string Topic { get; set; }
    public Dictionary<char, int> Modes { get; set; } = new();
    public Dictionary<string, string> Props { get; set; } = new();
}