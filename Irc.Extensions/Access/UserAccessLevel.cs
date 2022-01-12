namespace Irc.Extensions.Access;

public enum UserAccessLevel
{
    None = 0,
    ChatGuest = 1,
    ChatUser = 2,
    ChatMember = 3,
    ChatHost = 4,
    ChatOwner = 5,
    ChatGuide = 6,
    ChatSysop = 7,
    ChatSysopManager = 8,
    ChatAdministrator = 9,
    NoAccess = 10,
    ChatService = 11
}