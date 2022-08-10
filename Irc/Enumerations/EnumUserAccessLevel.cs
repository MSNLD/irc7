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
    Guide = 7,
    Sysop = 8,
    Administrator = 9,
    Service = 10
}