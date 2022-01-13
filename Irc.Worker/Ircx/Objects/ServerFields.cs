namespace Irc.Worker.Ircx.Objects;

public class ServerFields
{
    public string CreationDate;
    public string TimeZone;

    public int MaxUsers => 0;
    public int RegisteredUsers { get; private set; }
    public int InvisibleCount { get; private set; }
    public int UnknownConnections { get; private set; }
    public int OperatorCount { get; }
    public int IrcxVersion { get; }
    public string FullName { get; set; }
}