namespace Irc.Enumerations;

public enum EnumAccessLevel
{
    NONE = -2,
    DENY = -1,
    GRANT = 0,
    VOICE = 1,
    HOST = 2,
    OWNER = 3,
    All = 4
}