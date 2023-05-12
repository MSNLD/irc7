using System.Text.RegularExpressions;

namespace Irc.Helpers.CSharpTools;

public class RegularExpressions
{
    private Regex regex;

    public RegularExpressions(string pattern, bool ignoreCase)
    {
        regex = new Regex(pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
    }

    public static bool Match(string pattern, string text, bool ignoreCase)
    {
        var regex = new Regex(pattern, ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        var match = regex.Match(text);
        return match.Success;
    }

    public bool IsMatch(string text)
    {
        var match = regex.Match(text);
        return match.Success;
    }
}