﻿namespace Irc.Constants;

public static class Resources
{
    public static long epoch = 621355968000000000;
    public static string CommandAccess = "ACCESS";
    public static string CommandAdmin = "ADMIN";
    public static string CommandAway = "AWAY";
    public static string CommandAuth = "AUTH";
    public static string CommandEvent = "EVENT";
    public static string CommandGoto = "GOTO";
    public static string CommandInfo = "INFO";
    public static string CommandInvite = "INVITE";
    public static string CommandIrcvers = "IRCVERS";
    public static string CommandIsIrcx = "ISIRCX";
    public static string CommandIrcx = "IRCX";
    public static string CommandJoin = "JOIN";
    public static string CommandKick = "KICK";
    public static string CommandKill = "KILL";
    public static string CommandListx = "LISTX";
    public static string CommandMode = "MODE";
    public static string CommandNick = "NICK";
    public static string CommandNames = "NAMES";
    public static string CommandNotice = "NOTICE";
    public static string CommandPart = "PART";
    public static string CommandPing = "PING";
    public static string CommandPong = "PONG";
    public static string CommandProp = "PROP";
    public static string CommandPrivmsg = "PRIVMSG";
    public static string CommandQuit = "QUIT";
    public static string CommandTime = "TIME";
    public static string CommandTopic = "TOPIC";
    public static string CommandUser = "USER";
    public static string CommandUserhost = "USERHOST";
    public static string CommandVersion = "VERSION";
    public static string CommandWho = "WHO";
    public static string CommandWhois = "WHOIS";
    public static string CommandWhisper = "WHISPER";

    public static string CRLF = "\r\n";
    public static string PROP_NAME = "NAME";
    public static string PROP_TOPIC = "TOPIC";
    public static string PROP_LAG = "LAG";
    public static string S_OK = "OK";
    public static string IRC = "IRC";
    public static string Wildcard = "*";
    public static string Self = "$";
    public static string Home = "H";
    public static string Gone = "G";
    public static string Admin = "a";
    public static string ISIRCX = "ISIRCX";
    public static string PRIVMSG = "PRIVMSG";
    public static string NOTICE = "NOTICE";
    public static string CONNRESETBYPEER = "Connection reset by peer";
    public static string INPUTFLOODING = "Input flooding";
    public static string OUTPUTSATURATION = "Output Saturation";
    public static string PINGTIMEOUT = "Ping Timeout";
    public static string TimeRegionServerTime = "ST";
    public static string TimeRegionGMT = "GT";
    public static string TimeRegionEST = "ET";
    public static string Zero = "0";
    public static string One = "1";
    public static string DefaultUserLimit = "100";
    public static string DefaultCreateUserLimit = "50";
    public static string DefaultChannelModes = "ntl";
    public static string Raw471 = "471";
    public static string Raw473 = "473";
    public static string Raw474 = "474";
    public static string Raw475 = "475";
    public static string Raw556 = "556";
    public static string Raw557 = "557";
    public static string Raw812 = "812";
    public static string Raw817 = "817";
    public static string FlagOwner = ".";
    public static string FlagHost = "@";
    public static string FlagVoice = "+";
    public static string UserhostParticle = "=+";
    public static string enUS = "EN-US";
    public static string Service = "SERVICE";
    public static string Delete = "DELETE";
    public static string IRCXOptions = "*";


    public static string UserAccessGroupService = "Service Accounts";
    public static string UserAccessGroupAdministrators = "Chat Administrators";
    public static string UserAccessGroupSysopManagers = "Chat Sysop Managers";
    public static string UserAccessGroupSysops = "Chat Sysops";
    public static string UserAccessGroupGuides = "Chat Guides";

    public static string IrcOpNickMask = @"[\x41-\xFF\-0-9]+$";
    public static int MaxFieldLen = 64;

    public static long GetTime()
    {
        return DateTime.UtcNow.Ticks;
    }

    public static string GetFullTimeString(long creation)
    {
        var time = DateTime.FromBinary(creation);
        var TimeZone = time.GetDateTimeFormats('R')[0];
        TimeZone = TimeZone.Substring(TimeZone.LastIndexOf(' ') + 1);
        return time.ToString("\"\"MMM d yyyy\" at \"HH:mm:ss ") + TimeZone;
    }

    #region Channel Categories

    public static string ChannelCategoryTeens = "TN";
    public static string ChannelCategoryComputing = "CP";
    public static string ChannelCategoryEvents = "EV";
    public static string ChannelCategoryGeneral = "GN";
    public static string ChannelCategoryHealth = "HE";
    public static string ChannelCategoryCityChats = "GE";
    public static string ChannelCategoryEntertainment = "EA";
    public static string ChannelCategoryInterests = "II";
    public static string ChannelCategoryLifestyles = "LF";
    public static string ChannelCategoryMusic = "MU";
    public static string ChannelCategoryPeers = "PR";
    public static string ChannelCategoryNews = "NW";
    public static string ChannelCategoryReligion = "RL";
    public static string ChannelCategoryRomance = "RM";
    public static string ChannelCategorySports = "SP";
    public static string ChannelCategoryUnlisted = "UL";

    #endregion

    #region Channel Country & Language

    public static string ChannelCountryLanguageENUS = "EN-US";
    public static string ChannelCountryLanguageENCA = "EN-CA";
    public static string ChannelCountryLanguageENGB = "EN-GB";
    public static string ChannelCountryLanguageENUK = "EN-UK";
    public static string ChannelCountryLanguageFRCA = "FR-CA";
    public static string ChannelCountryLanguageENAU = "EN-AU";

    #endregion

    #region User Properties

    public static string UserPropOid = "OID";
    public static string UserPropName = "NAME";
    public static string UserPropRole = "ROLE";
    public static string UserPropMsnProfile = "MSNPROFILE"; //
    public static string UserPropMsnRegCookie = "MSNREGCOOKIE";
    public static string UserPropNickname = "NICK";
    public static string UserPropPuid = "PUID";
    public static string UserPropIrcvers = "IRCVERS";
    public static string UserPropClient = "CLIENT";

    #endregion

    #region Channel Properties

    public static string ChannelPropName = "NAME";
    public static string ChannelPropTopic = "TOPIC";
    public static string ChannelPropLag = "LAG";
    public static string ChannelPropLanguage = "LANGUAGE";
    public static string ChannelPropSubject = "SUBJECT";
    public static string ChannelPropMemberkey = "MEMBERKEY";
    public static string ChannelPropOwnerkey = "OWNERKEY";
    public static string ChannelPropHostkey = "HOSTKEY";
    public static string ChannelPropCreation = "CREATION";
    public static string ChannelPropOID = "OID";
    public static string ChannelPropPICS = "PICS";
    public static string ChannelPropOnJoin = "ONJOIN";
    public static string ChannelPropOnPart = "ONPART";
    public static string ChannelPropClientGuid = "CLIENTGUID";

    #endregion

    #region Server Events

    public static string EventLevelUser = "USER";
    public static string EventLevelChannel = "CHANNEL";
    public static string EventLevelMember = "MEMBER";
    public static string EventLevelServer = "SERVER";
    public static string EventLevelConnect = "CONNECT";
    public static string EventLevelSocket = "SOCKET";

    #endregion

    #region Channel Access

    public static string AccessLevelOwner = "OWNER";
    public static string AccessLevelHost = "HOST";
    public static string AccessLevelVoice = "VOICE";
    public static string AccessLevelDeny = "DENY";
    public static string AccessLevelGrant = "GRANT";

    public static string AccessEntryOperatorAdd = "ADD";
    public static string AccessEntryOperatorDelete = "DELETE";
    public static string AccessEntryOperatorClear = "CLEAR";
    public static string AccessEntryOperatorList = "LIST";

    #endregion

    #region Channel Resources

    public static char ChannelModePrivate = 'p';
    public static char ChannelModeSecret = 's';
    public static char ChannelModeModerated = 'm';
    public static char ChannelModeNoExtern = 'n';
    public static char ChannelModeTopicOp = 't';
    public static char ChannelModeInvite = 'i';
    public static char ChannelModeUserLimit = 'l';
    public static char ChannelModeBan = 'b';
    public static char ChannelModeKey = 'k';

    public static char MemberModeOwner = 'q';
    public static char MemberModeHost = 'o';
    public static char MemberModeVoice = 'v';

    public static char MemberModeFlagOwner = '.';
    public static char MemberModeFlagHost = '@';
    public static char MemberModeFlagVoice = '+';

    #endregion

    #region "User Modes"

    public static char UserModeAdmin = 'a';
    public static char UserModeOper = 'o';
    public static char UserModeOwner = 'q';
    public static char UserModeInvisible = 'i';
    public static char UserModeServerNotice = 's';
    public static char UserModeWallops = 'w';

    #endregion

    #region "Regular Expressions"

    public static string StandardIRC = @"[(\x00-\x2C)(\x2E-\x2F)(\x3A-\x40)]{1}|\\N|\\R|\\0|\\T";
    public static string GenericProps = @"[\x00-\x19]{1}";
    public static string JoinPartProp = @"[\x00]{1}";
    public static string IrcChannelRegex = @"#[\x21-\x2B,\x2D-\xFF]{1,200}";
    public static string IrcxChannelRegex = @"%#[\x21-\x2B,\x2D-\xFF]{1,200}";
    public static string GuestNicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";
    public static string NicknameMask = @"^(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF][\x41-\xFF\-0-9]*$";

    #endregion
}