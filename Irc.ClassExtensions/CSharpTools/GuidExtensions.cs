namespace Irc.Helpers.CSharpTools;

public static class GuidExtensions
{
    public static string ToUnformattedString(this Guid guid)
    {
        return guid.ToString("N");
    }
}