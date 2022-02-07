namespace Irc.Extensions;

public static class ExtendedResources
{
    public static char ChannelModeHidden = 'h';
    public static char ChannelModeAuthOnly = 'a'; // a - NTLM auth only
    public static char ChannelModeCloneable = 'd';
    public static char ChannelModeClone = 'e';
    public static char ChannelModeProfanity = 'f';
    public static char ChannelModeRegistered = 'r';
    public static char ChannelModeNoWhisper = 'w';
    public static char ChannelModeService = 'z';
    public static char ChannelModeKnock = 'u';
    public static char ChannelModeAuditorium = 'x';

    public static char MemberModeOwner = 'q';
    public static char MemberModeFlagOwner = '.';

    public static char UserModePasskey = 'h';
    public static char UserModeIrcx = 'x';
    public static char UserModeGag = 'z';
}