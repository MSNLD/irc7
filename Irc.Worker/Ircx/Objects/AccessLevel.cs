namespace Irc.Worker.Ircx.Objects;

public class AccessLevel
{
    public static AccessLevel None = new(EnumAccessLevel.NONE, string.Empty);
    public EnumAccessLevel Level;
    public string LevelText;

    public AccessLevel(EnumAccessLevel Level, string LevelText)
    {
        this.Level = Level;
        this.LevelText = LevelText;
    }
}