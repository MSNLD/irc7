namespace Irc.Models.Enumerations;

public enum EnumUserAccessLevel
{
    NoAccess = -1,
    None = 0,
    Guest = 1,
    Member = 2,
    Guide = 7,
    Sysop = 8,
    Administrator = 9,
    Service = 10
}