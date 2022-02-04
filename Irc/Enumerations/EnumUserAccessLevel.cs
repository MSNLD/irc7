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
    ChatGuide = 7,
    ChatSysop = 8,
    ChatSysopManager = 9,
    ChatAdministrator = 10,
    ChatService = 11
}