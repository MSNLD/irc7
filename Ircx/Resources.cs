using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;

namespace Core
{
    static class Resources
    {
        public static long epoch = 621355968000000000;
        public static String8 CommandAccess = "ACCESS";
        public static String8 CommandAdmin = "ADMIN";
        public static String8 CommandAway = "AWAY";
        public static String8 CommandAuth = "AUTH";
        public static String8 CommandEvent = "EVENT";
        public static String8 CommandGoto = "GOTO";
        public static String8 CommandInfo = "INFO";
        public static String8 CommandInvite = "INVITE";
        public static String8 CommandIrcvers = "IRCVERS";
        public static String8 CommandIsIrcx = "ISIRCX";
        public static String8 CommandIrcx = "IRCX";
        public static String8 CommandJoin = "JOIN";
        public static String8 CommandKick = "KICK";
        public static String8 CommandKill = "KILL";
        public static String8 CommandListx = "LISTX";
        public static String8 CommandMode = "MODE";
        public static String8 CommandNick = "NICK";
        public static String8 CommandNames = "NAMES";
        public static String8 CommandNotice = "NOTICE";
        public static String8 CommandPart = "PART";
        public static String8 CommandPing = "PING";
        public static String8 CommandPong = "PONG";
        public static String8 CommandProp = "PROP";
        public static String8 CommandPrivmsg = "PRIVMSG";
        public static String8 CommandQuit = "QUIT";
        public static String8 CommandTime = "TIME";
        public static String8 CommandTopic = "TOPIC";
        public static String8 CommandUser = "USER";
        public static String8 CommandUserhost = "USERHOST";
        public static String8 CommandVersion = "VERSION";
        public static String8 CommandWho = "WHO";
        public static String8 CommandWhois = "WHOIS";
        public static String8 CommandWhisper = "WHISPER";

        public static String8 CRLF = "\r\n";
        public static String8 PROP_NAME = "NAME";
        public static String8 PROP_TOPIC = "TOPIC";
        public static String8 PROP_LAG = "LAG";
        public static String8 S_OK = "OK";
        public static String8 IRC = "IRC";
        public static String8 Null = "";
        public static String8 Wildcard = "*";
        public static String8 Self = "$";
        public static String8 Home = "H";
        public static String8 Gone = "G";
        public static String8 Admin = "a";
        public static String8 ISIRCX = "ISIRCX";
        public static String8 PRIVMSG = "PRIVMSG";
        public static String8 NOTICE = "NOTICE";
        public static String8 CONNRESETBYPEER = "Connection reset by peer";
        public static String8 INPUTFLOODING = "Input flooding";
        public static String8 OUTPUTSATURATION = "Output Saturation";
        public static String8 PINGTIMEOUT = "Ping Timeout";
        public static String8 TimeRegionServerTime = "ST";
        public static String8 TimeRegionGMT = "GT";
        public static String8 TimeRegionEST = "ET";
        public static String8 Zero = "0";
        public static String8 One = "1";
        public static String8 DefaultUserLimit = "100";
        public static String8 DefaultCreateUserLimit = "50";
        public static String8 DefaultChannelModes = "ntl";
        public static String8 Raw471 = "471";
        public static String8 Raw473 = "473";
        public static String8 Raw474 = "474";
        public static String8 Raw475 = "475";
        public static String8 Raw556 = "556";
        public static String8 Raw557 = "557";
        public static String8 Raw812 = "812";
        public static String8 Raw817 = "817";
        public static String8 FlagOwner = ".";
        public static String8 FlagHost = "@";
        public static String8 FlagVoice = "+";
        public static String8 UserhostParticle = "=+";
        public static String8 enUS = "EN-US";
        public static String8 Service = "SERVICE";
        public static String8 Delete = "DELETE";
        public static String8 IRCXOptions = "*";


        public static String8 UserAccessGroupService = "Service Accounts";
        public static String8 UserAccessGroupAdministrators = "Chat Administrators";
        public static String8 UserAccessGroupSysopManagers = "Chat Sysop Managers";
        public static String8 UserAccessGroupSysops = "Chat Sysops";
        public static String8 UserAccessGroupGuides = "Chat Guides";

        #region Channel Categories
        public static String8 ChannelCategoryTeens = "TN";
        public static String8 ChannelCategoryComputing = "CP";
        public static String8 ChannelCategoryEvents = "EV";
        public static String8 ChannelCategoryGeneral = "GN";
        public static String8 ChannelCategoryHealth = "HE";
        public static String8 ChannelCategoryCityChats = "GE";
        public static String8 ChannelCategoryEntertainment = "EA";
        public static String8 ChannelCategoryInterests = "II";
        public static String8 ChannelCategoryLifestyles = "LF";
        public static String8 ChannelCategoryMusic = "MU";
        public static String8 ChannelCategoryPeers = "PR";
        public static String8 ChannelCategoryNews = "NW";
        public static String8 ChannelCategoryReligion = "RL";
        public static String8 ChannelCategoryRomance = "RM";
        public static String8 ChannelCategorySports = "SP";
        public static String8 ChannelCategoryUnlisted = "UL";
        
        #endregion

        #region Channel Country & Language
        public static String8 ChannelCountryLanguageENUS = "EN-US";
        public static String8 ChannelCountryLanguageENCA = "EN-CA";
        public static String8 ChannelCountryLanguageENGB = "EN-GB";
        public static String8 ChannelCountryLanguageENUK = "EN-UK";
        public static String8 ChannelCountryLanguageFRCA = "FR-CA";
        public static String8 ChannelCountryLanguageENAU = "EN-AU";
        #endregion

        #region User Properties
        public static String8 UserPropName = "NAME";
        public static String8 UserPropRole = "ROLE";
        public static String8 UserPropMsnProfile = "MSNPROFILE"; //
        public static String8 UserPropMsnRegCookie = "MSNREGCOOKIE";
        public static String8 UserPropNickname = "NICK";
        public static String8 UserPropPuid = "PUID";
        public static String8 UserPropIrcvers = "IRCVERS";
        public static String8 UserPropClient = "CLIENT";
        #endregion

        #region Channel Properties
        public static String8 ChannelPropName = "NAME";
        public static String8 ChannelPropTopic = "TOPIC";
        public static String8 ChannelPropLag = "LAG";
        public static String8 ChannelPropLanguage = "LANGUAGE";
        public static String8 ChannelPropSubject = "SUBJECT";
        public static String8 ChannelPropMemberkey = "MEMBERKEY";
        public static String8 ChannelPropOwnerkey = "OWNERKEY";
        public static String8 ChannelPropHostkey = "HOSTKEY";
        public static String8 ChannelPropCreation = "CREATION";
        public static String8 ChannelPropOID = "OID";
        public static String8 ChannelPropPICS = "PICS";
        public static String8 ChannelPropOnJoin = "ONJOIN";
        public static String8 ChannelPropOnPart = "ONPART";
        public static String8 ChannelPropClientGuid = "CLIENTGUID";
        #endregion

        #region Server Events
        public static String8 EventLevelUser = "USER";
        public static String8 EventLevelChannel = "CHANNEL";
        public static String8 EventLevelMember = "MEMBER";
        public static String8 EventLevelServer = "SERVER";
        public static String8 EventLevelConnect = "CONNECT";
        public static String8 EventLevelSocket = "SOCKET";
        #endregion

        #region Channel Access
        public static String8 AccessLevelOwner = "OWNER";
        public static String8 AccessLevelHost = "HOST";
        public static String8 AccessLevelVoice = "VOICE";
        public static String8 AccessLevelDeny = "DENY";
        public static String8 AccessLevelGrant = "GRANT";

        public static String8 AccessEntryOperatorAdd = "ADD";
        public static String8 AccessEntryOperatorDelete = "DELETE";
        public static String8 AccessEntryOperatorClear = "CLEAR";
        public static String8 AccessEntryOperatorList = "LIST";
        #endregion

        #region Channel Resources
        public static byte ChannelModeCharAuthOnly = (byte)'a';
        //public static byte ChannelModeCharCloneable = (byte)'d';
        //public static byte ChannelModeCharClone = (byte)'e';
        //public static byte ChannelModeCharExpiry = (byte)'e';
        public static byte ChannelModeCharProfanity = (byte)'f';
        public static byte ChannelModeCharOnStage = (byte)'g';
        public static byte ChannelModeCharHidden = (byte)'h';
        public static byte ChannelModeCharKey = (byte)'k';
        public static byte ChannelModeCharInvite = (byte)'i';
        public static byte ChannelModeCharUserLimit = (byte)'l';
        public static byte ChannelModeCharModerated = (byte)'m';
        public static byte ChannelModeCharNoExtern = (byte)'n';
        public static byte ChannelModeCharPrivate = (byte)'p';
        public static byte ChannelModeCharRegistered = (byte)'r';
        public static byte ChannelModeCharSecret = (byte)'s';
        public static byte ChannelModeCharSubscriber = (byte)'S';
        public static byte ChannelModeCharTopicOp = (byte)'t';
        public static byte ChannelModeCharKnock = (byte)'u';
        public static byte ChannelModeCharNoWhisper = (byte)'w';
        public static byte ChannelModeCharNoGuestWhisper = (byte)'W';
        public static byte ChannelModeCharAuditorium = (byte)'x';

        public static byte ChannelUserModeCharOwner = (byte)'q';
        public static byte ChannelUserModeCharHost = (byte)'o';
        public static byte ChannelUserModeCharVoice = (byte)'v';
        public static byte ChannelUserModeCharBan = (byte)'b';

        public static byte ChannelUserFlagOwner = (byte)'.';
        public static byte ChannelUserFlagHost = (byte)'@';
        public static byte ChannelUserFlagVoice = (byte)'+';

        public static String8 ChannelModeDefault = "ntl50";
        #endregion

        #region "User Modes"
        public static byte UserModeCharAdmin = (byte)'a';
        public static byte UserModeCharOper = (byte)'o';
        public static byte UserModeCharInvisible = (byte)'i';
        public static byte UserModeCharServerNotice = (byte)'s';
        public static byte UserModeCharWallops = (byte)'w';
        public static byte UserModeCharIrcx = (byte)'x';
        public static byte UserModeCharGag = (byte)'z';
        public static byte UserModePasskey = (byte)'h';
        #endregion

        #region "Regular Expressions"
        public static string StandardIRC = @"[(\x00-\x2C)(\x2E-\x2F)(\x3A-\x40)]{1}|\\N|\\R|\\0|\\T";
        public static string GenericProps = @"[\x00-\x19]{1}";
        public static string JoinPartProp = @"[\x00]{1}";
        public static string RegExNickname = @".+";
        public static string ChannelRegEx = @"%#[\x21-\x2B,\x2D-\xFF]{1,200}";
        #endregion

        public static long GetTime()
        {
            return DateTime.UtcNow.Ticks;
        }
        public static string GetFullTimeString(long creation)
        {
            DateTime time = DateTime.FromBinary(creation);
            string TimeZone = time.GetDateTimeFormats('R')[0];
            TimeZone = TimeZone.Substring(TimeZone.LastIndexOf(' ') + 1);
            return time.ToString("\"\"MMM d yyyy\" at \"HH:mm:ss ") + TimeZone;
        }
    }
}
