namespace Irc.Enumerations;

public enum EnumUserAccessLevel
{
    NoAccess = -1,
    None = 0,
    ChatGuest = 1,
    ChatUser = 2,
    ChatMember = 3,
    ChatVoice = 4,
    ChatHost = 5,
    ChatOwner = 6,
    Host = 7,
    Guide = 8,
    Sysop = 9,
    SysopManager = 10,
    Administrator = 11,
    Service = 12
}