using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Ircx.Objects;

namespace Core.Ircx
{
    public static class Raw
    {
        public static string IRCX_CLOSINGLINK(Server server, User user) => $"ERROR :Closing Link: {user}[%s] (%s)";
        public static string IRCX_CLOSINGLINK_007_SYSTEMKILL(Server server, User user) => $"ERROR :Closing Link: {user}[%s] 007 (Killed by system operator)";
        public static string IRCX_CLOSINGLINK_008_INPUTFLOODING(Server server, User user) => $"ERROR :Closing Link: {user}[%s] 008 (Input flooding)";
        public static string IRCX_CLOSINGLINK_009_OUTPUTSATURATION(Server server, User user) => $"ERROR :Closing Link: {user}[%s] 009 (Output Saturation)";
        public static string IRCX_CLOSINGLINK_011_PINGTIMEOUT(Server server, User user) => $"ERROR :Closing Link: {user}[%s] 011 (Ping timeout)";

        public static string RPL_AUTH_INIT(Server server, User user) => $"AUTH %s I :%s";
        public static string RPL_AUTH_SEC_REPLY(Server server, User user) => $"AUTH %s S :%s";
        public static string RPL_AUTH_SUCCESS(Server server, User user) => $"AUTH %s * %s %o";
        public static string RPL_JOIN_IRC(Server server, User user, Channel channel) => $":{user.Address} JOIN :{channel}";
        public static string RPL_JOIN_MSN(Server server, User user, Channel channel) => $":{user.Address} JOIN %S :{channel}";
        public static string RPL_PART_IRC(Server server, User user, Channel channel) => $":{user.Address} PART {channel}";
        public static string RPL_QUIT_IRC(Server server, User user) => $":{user.Address} QUIT :%s";
        public static string RPL_MODE_IRC(Server server, User user) => $":{user.Address} MODE %s %s";
        public static string RPL_TOPIC_IRC(Server server, User user, Channel channel) => $":{user.Address} TOPIC {channel} :%s";
        public static string RPL_PROP_IRCX(Server server, User user, Channel channel) => $":{user.Address} PROP {channel} %s :%s";
        public static string RPL_KICK_IRC(Server server, User user, Channel channel) => $":{user.Address} KICK {channel} %s :%s";
        public static string RPL_KILL_IRC(Server server, User user) => $":{user.Address} KILL %s :%s";
        public static string RPL_SERVERKILL_IRC(Server server, User user) => $":{server} KILL %s :%s";
        public static string RPL_KILLED(Server server, User user) => $":%s KILLED";
        public static string RPL_MSG_CHAN(Server server, User user, Channel channel) => $":{user.Address} %s {channel} :%s";
        public static string RPL_MSG_USER(Server server, User user) => $":{user.Address} %s %s :%s";
        public static string RPL_CHAN_MSG(Server server, User user) => $":%s %s %s :%s";
        public static string RPL_CHAN_WHISPER(Server server, User user, Channel channel) => $":{user.Address} WHISPER {channel} %s :%s";
        public static string RPL_EPRIVMSG_CHAN(Server server, User user, Channel channel) => $":{user.Address} EPRIVMSG {channel} :%s";
        public static string RPL_NOTICE_USER(Server server, User user) => $":{user.Address} NOTICE %s :%s";
        public static string RPL_PRIVMSG_USER(Server server, User user) => $":{user.Address} PRIVMSG %s :%s";
        public static string RPL_NOTICE_CHAN(Server server, User user, Channel channel) => $":{user.Address} NOTICE {channel} :%s";
        public static string RPL_PRIVMSG_CHAN(Server server, User user, Channel channel) => $":{user.Address} PRIVMSG {channel} :%s";
        public static string RPL_INVITE(Server server, User user, Channel channel) => $":{user.Address} INVITE %s %s :{channel}";
        public static string RPL_KNOCK_CHAN(Server server, User user, Channel channel) => $":{user.Address} KNOCK {channel} %s";
        public static string RPL_NICK(Server server, User user) => $":{user.Address} NICK %s";
        public static string RPL_PONG(Server server, User user) => $"PONG {server} :{user}";
        public static string RPL_PONG_CLIENT(Server server, User user) => $"PONG :%s";
        public static string RPL_PING(Server server, User user) => $"PING :{server}";
        public static string RPL_SERVICE_DATA(Server server, User user) => $":{server} SERVICE %s %s"; //e.g. :SERVERNAMEs SERVICE DELETE CHANNEL %#Channel


        public static string IRCX_RPL_WELCOME_001(Server server, User user) => $":{server} 001 {user} :Welcome to the %s server {user}";
        public static string IRCX_RPL_WELCOME_002(Server server, User user, Version version) => $":{server} 002 {user} :Your host is {server}, running version {version.Major}.{version.Minor}.{version.Build}";
        public static string IRCX_RPL_WELCOME_003(Server server, User user) => $":{server} 003 {user} :This server was created %s";
        public static string IRCX_RPL_WELCOME_004(Server server, User user, Version version) => $":{server} 004 {user} {server} {version.Major}.{version.Minor}.{version.Build} aioxz abcdefhiklmnoprstuvxyz";
        public static string IRCX_RPL_WELCOME_005(Server server, User user) => $":{server} 005 {user} IRCX PREFIX=(qov).@+ CHANTYPES=%# CHANLIMIT=%#:1";

        public static string IRCX_RPL_UMODEIS_221(Server server, User user) => $":{server} 221 {user} %s";

        public static string IRCX_RPL_LUSERCLIENT_251(Server server, User user) => $":{server} 251 {user} :There are %d users and %d invisible on %d servers";
        public static string IRCX_RPL_LUSEROP_252(Server server, User user) => $":{server} 252 {user} %d :operator(s) online";
        public static string IRCX_RPL_LUSERUNKNOWN_253(Server server, User user) => $":{server} 253 {user} %d :unknown connection(s)";
        public static string IRCX_RPL_LUSERCHANNELS_254(Server server, User user) => $":{server} 254 {user} %d :channels formed";
        public static string IRCX_RPL_LUSERME_255(Server server, User user) => $":{server} 255 {user} :I have %d clients and %d servers";

        public static string IRCX_RPL_ADMINME_256(Server server, User user) => $":{server} 256 {user} :Administrative info about {server}";
        public static string IRCX_RPL_ADMINLOC1_257(Server server, User user) => $":{server} 257 {user} :%s";
        public static string IRCX_RPL_ADMINLOC1_258(Server server, User user) => $":{server} 258 {user} :%s";
        public static string IRCX_RPL_ADMINEMAIL_259(Server server, User user) => $":{server} 259 {user} :%s";

        public static string IRCX_RPL_LUSERS_265(Server server, User user) => $":{server} 265 {user} :Current local users: %d Max: %d";
        public static string IRCX_RPL_GUSERS_266(Server server, User user) => $":{server} 266 {user} :Current global users: %d Max: %d";

        public static string IRCX_RPL_AWAY_301(Server server, User user) => $":{server} 301 {user} %s :%s";
        public static string IRCX_RPL_USERHOST_302(Server server, User user) => $":{server} 302 {user} :%s";
        public static string IRCX_RPL_UNAWAY_305(Server server, User user) => $":{server} 305 {user} :You are no longer marked as being away";
        public static string IRCX_RPL_NOWAWAY_306(Server server, User user) => $":{server} 306 {user} :You have been marked as being away";
        public static string IRCX_RPL_WHOISUSER_311(Server server, User user) => $":{server} 311 {user} %s %s %s * :%s";
        public static string IRCX_RPL_WHOISSERVER_312(Server server, User user) => $":{server} 312 {user} %s %s :%s";
        public static string IRCX_RPL_WHOISOPERATOR_313A(Server server, User user) => $":{server} 313 {user} %s :is an IRC administrator";
        public static string IRCX_RPL_WHOISOPERATOR_313O(Server server, User user) => $":{server} 313 {user} %s :is an IRC operator";
        public static string IRCX_RPL_ENDOFWHO_315(Server server, User user) => $":{server} 315 {user} %s :End of /WHO list";
        public static string IRCX_RPL_WHOISIDLE_317(Server server, User user) => $":{server} 317 {user} %s %d %d :seconds idle, signon time";
        public static string IRCX_RPL_ENDOFWHOIS_318(Server server, User user) => $":{server} 318 {user} %s :End of /WHOIS list";
        public static string IRCX_RPL_WHOISCHANNELS_319(Server server, User user) => $":{server} 319 {user} %s :%s";
        public static string IRCX_RPL_WHOISCHANNELS_319X(Server server, User user) => $":{server} 319 {user} %s :";
        public static string IRCX_RPL_WHOISIP_320(Server server, User user) => $":{server} 320 {user} %s :from IP %s";
        public static string IRCX_RPL_MODE_324(Server server, User user, Channel channel) => $":{server} 324 {user} {channel} %s";
        public static string IRCX_RPL_NOTOPIC_331(Server server, User user, Channel channel) => $":{server} 331 {user} {channel} :No topic is set";
        public static string IRCX_RPL_TOPIC_332(Server server, User user, Channel channel) => $":{server} 332 {user} {channel} :%s";
        public static string IRCX_RPL_VERSION_351(Server server, User user) => $":{server} 351 {user} %d.%d.%d {server} :{server} %d.%d";
        public static string IRCX_RPL_WHOREPLY_352(Server server, User user) => $":{server} 352 {user} %s %s %s {server} %s %s%s%s :0 %s";
        public static string IRCX_RPL_NAMEREPLY_353(Server server, User user, Channel channel) => $":{server} 353 {user}(Server server, User user) => {channel} :%s";
        public static string IRCX_RPL_NAMEREPLY_353X(Server server, User user, Channel channel) => $":{server} 353 {user}(Server server, User user) => {channel} :";
        public static string IRCX_RPL_ENDOFNAMES_366(Server server, User user, Channel channel) => $":{server} 366 {user} %s :End of /NAMES list.";
        public static string IRCX_RPL_BANLIST_367(Server server, User user, Channel channel) => $":{server} 367 {user} {channel} %s";
        public static string IRCX_RPL_ENDOFBANLIST_368(Server server, User user, Channel channel) => $":{server} 368 {user} {channel} :End of Channel Ban List";
        public static string IRCX_RPL_RPL_INFO_371(Server server, User user) => $":{server} 371 {user} :On-line since %s";
        public static string IRCX_RPL_RPL_INFO_371_VERS(Server server, User user) => $":{server} 371 {user} :%s %d.%d";
        public static string IRCX_RPL_RPL_MOTD_372(Server server, User user) => $":{server} 372 {user} :- %s";
        public static string IRCX_RPL_RPL_ENDOFINFO_374(Server server, User user) => $":{server} 374 {user} :End of /INFO list";
        public static string IRCX_RPL_RPL_MOTDSTART_375(Server server, User user) => $":{server} 375 {user} :- {server} Message of the Day -";
        public static string IRCX_RPL_RPL_ENDOFMOTD_376(Server server, User user) => $":{server} 376 {user} :End of /MOTD command";

        public static string IRCX_RPL_YOUREOPER_381(Server server, User user) => $":{server} 381 {user} :You are now an IRC operator";
        public static string IRCX_RPL_YOUREADMIN_386(Server server, User user) => $":{server} 386 {user} :You are now an IRC administrator";
        public static string IRCX_RPL_TIME_391(Server server, User user) => $":{server} 391 {user} {server} :%S";

        public static string IRCX_ERR_NOSUCHNICK_401_N(Server server, User user) => $":{server} 401 {user} %s :No such nick";
        public static string IRCX_ERR_NOSUCHNICK_401(Server server, User user) => $":{server} 401 {user} %s :No such nick/channel";
        public static string IRCX_ERR_NOSUCHCHANNEL_403(Server server, User user) => $":{server} 403 {user} %s :No such channel";
        public static string IRCX_ERR_CANNOTSENDTOCHAN_404(Server server, User user) => $":{server} 404 {user} %s :Cannot send to channel";
        public static string IRCX_ERR_CANNOTSENDTOCHAN_404X(Server server, User user, Channel channel) => $":{server} 404 {user} {channel} :Cannot send to channel";
        public static string IRCX_ERR_TOOMANYCHANNELS_405(Server server, User user, Channel channel) => $":{server} 405 {user} {channel} :You have joined too many channels";
        public static string IRCX_ERR_NOORIGIN_409(Server server, User user) => $":{server} 409 {user} :No origin specified";
        public static string IRC_ERR_NORECIPIENT_411(Server server, User user) => $":{server} 411 {user} :No recipient given (%s)";
        public static string IRC_ERR_NOTEXT_412(Server server, User user) => $":{server} 412 {user} :No text to send (%s)";
        public static string IRCX_ERR_UNKNOWNCOMMAND_421(Server server, User user) => $":{server} 421 {user} %s :Unknown command";
        public static string IRCX_ERR_UNKNOWNCOMMAND_421_T(Server server, User user) => $":{server} 421 {user} %s :String parameter must be 160 chars or less";
        public static string IRCX_ERR_NOMOTD_422(Server server, User user) => $":{server} 422 {user} :MOTD File is missing";
        public static string IRCX_ERR_ERRONEOUSNICK_432(Server server, User user) => $":{server} 432 %s :Erroneous nickname";
        public static string IRCX_ERR_NICKINUSE_433(Server server, User user) => $":{server} 433 * %s :Nickname is already in use";
        public static string IRCX_ERR_NONICKCHANGES_439(Server server, User user) => $":{server} 439 {user} %s :Nick name changes not permitted.";
        public static string IRCX_ERR_NOTONCHANNEL_442(Server server, User user) => $":{server} 442 {user} %s :You're not on that channel";
        public static string IRCX_ERR_USERONCHANNEL_443(Server server, User user) => $":{server} 443 {user} %s %s :is already on channel";
        public static string IRCX_ERR_NOTREGISTERED_451(Server server, User user) => $":{server} 451 {user} :You have not registered";
        public static string IRCX_ERR_NEEDMOREPARAMS_461(Server server, User user) => $":{server} 461 {user} %s :Not enough parameters";
        public static string IRCX_ERR_ALREADYREGISTERED_462(Server server, User user) => $":{server} 462 {user} :You may not reregister";
        public static string IRCX_ERR_KEYSET_467(Server server, User user, Channel channel) => $":{server} 467 {user} {channel} :Channel key already set";
        public static string IRCX_ERR_CHANNELISFULL_471(Server server, User user, Channel channel) => $":{server} 471 {user} {channel} :Cannot join channel (+l)";
        public static string IRCX_ERR_UNKNOWNMODE_472(Server server, User user) => $":{server} 472 {user} %l :is unknown mode char to me";
        public static string IRCX_ERR_INVITEONLYCHAN_473(Server server, User user, Channel channel) => $":{server} 473 {user} {channel} :Cannot join channel (+i)";
        public static string IRCX_ERR_BANNEDFROMCHAN_474(Server server, User user, Channel channel) => $":{server} 474 {user} {channel} :Cannot join channel (+b)";
        public static string IRCX_ERR_BADCHANNELKEY_475(Server server, User user, Channel channel) => $":{server} 475 {user} {channel} :Cannot join channel (+k)";
        public static string IRCX_ERR_NOPRIVILEGES_481(Server server, User user, Channel channel) => $":{server} 481 {user} :Permission Denied - You're not an IRC operator";
        public static string IRCX_ERR_CHANOPRIVSNEEDED_482(Server server, User user, Channel channel) => $":{server} 482 {user} {channel} :You're not channel operator";
        public static string IRCX_ERR_CHANQPRIVSNEEDED_485(Server server, User user, Channel channel) => $":{server} 485 {user} {channel} :You're not channel owner";

        public static string IRCX_ERR_USERSDONTMATCH_502(Server server, User user) => $":{server} 502 {user} :Cant change mode for other users";
        public static string IRCX_ERR_OPTIONUNSUPPORTED_555(Server server, User user) => $":{server} 555 {user} %s :Server option for this command is not supported.";
        public static string IRCX_ERR_AUTHONLYCHAN_556(Server server, User user, Channel channel) => $":{server} 556 {user} {channel} :Only authenticated users may join channel.";
        public static string IRCX_ERR_SECUREONLYCHAN_557(Server server, User user, Channel channel) => $":{server} 557 {user} {channel} :Only secure users may join channel.";

        public static string IRCX_RPL_FINDS_613(Server server, User user) => $":{server} 613 {user} :%s %s";
        public static string IRCX_RPL_LISTRSTART_614(Server server, User user) => $":{server} 811 {user} :Start of ListR";
        public static string IRCX_RPL_LISTRLIST_614(Server server, User user, Channel channel) => $":{server} 812 {user} {channel} %d %s :%s";
        public static string IRCX_RPL_LISTREND_614(Server server, User user) => $":{server} 817 {user} :End of ListR";
        public static string IRCX_RPL_YOUREGUIDE_629(Server server, User user) => $":{server} 629 {user} :You are now an IRC guide";

        public static string IRC2_RPL_WHOISSECURE_671(Server server, User user) => $":{server} 671 {user} %s :is using a secure connection";

        public static string IRCX_RPL_FINDS_NOSUCHCAT_701(Server server, User user) => $":{server} 701 {user} :Category not found";
        public static string IRCX_RPL_FINDS_NOTFOUND_702(Server server, User user) => $":{server} 702 {user} :Channel not found";
        public static string IRCX_RPL_FINDS_DOWN_703(Server server, User user) => $":{server} 703 {user} :Server down. Retry later.";
        public static string IRCX_RPL_FINDS_CHANNELEXISTS_705(Server server, User user) => $":{server} 705 {user} :Channel with same name exists";
        public static string IRCX_RPL_FINDS_INVALIDCHANNEL_706(Server server, User user) => $":{server} 706 {user} :Channel name is not valid";

        public static string IRCX_RPL_IRCX_800(Server server, User user) => $":{server} 800 {user} %d %d %s %d %s";
        public static string IRCX_RPL_ACCESSADD_801(Server server, User user) => $":{server} 801 {user} %s %s %s %d %s :%s";
        public static string IRCX_RPL_ACCESSDELETE_802(Server server, User user) => $":{server} 802 {user} %s %s %s %d %s :%s";
        public static string IRCX_RPL_ACCESSSTART_803(Server server, User user) => $":{server} 803 {user} %s :Start of access entries";
        public static string IRCX_RPL_ACCESSLIST_804(Server server, User user) => $":{server} 804 {user} %s %s %s %d %s :%s";
        public static string IRCX_RPL_ACCESSEND_805(Server server, User user) => $":{server} 805 {user} %s :End of access entries";
        public static string IRCX_RPL_EVENTADD_806(Server server, User user) => $":{server} 806 {user} %s %s";
        public static string IRCX_RPL_EVENTDEL_807(Server server, User user) => $":{server} 807 {user} %s %s";
        public static string IRCX_RPL_EVENTSTART_808(Server server, User user) => $":{server} 808 {user} %s :Start of events";
        public static string IRCX_RPL_EVENTLIST_809(Server server, User user) => $":{server} 809 {user} %s %s";
        public static string IRCX_RPL_EVENTEND_810(Server server, User user) => $":{server} 810 {user} %s :End of events";
        public static string IRCX_RPL_LISTXSTART_811(Server server, User user) => $":{server} 811 {user} :Start of ListX";
        public static string IRCX_RPL_LISTXLIST_812(Server server, User user) => $":{server} 812 {user} %s %s %d %d :%s";
        public static string IRCX_RPL_LISTXPICS_813(Server server, User user) => $":{server} 813 {user} :%s";
        public static string IRCX_RPL_LISTXTRUNC_816(Server server, User user) => $":{server} 816 {user} :Truncation of ListX";
        public static string IRCX_RPL_LISTXEND_817(Server server, User user) => $":{server} 817 {user} :End of ListX";

        public static string IRCX_RPL_PROPLIST_818(Server server, User user) => $":{server} 818 {user} %s %s :%s";
        public static string IRCX_RPL_PROPEND_819(Server server, User user) => $":{server} 819 {user} %s :End of properties";
        public static string IRCX_RPL_ACCESSCLEAR_820(Server server, User user) => $":{server} 820 {user} %s %s :Clear";
        public static string IRCX_RPL_USERUNAWAY_821(Server server, User user) => $":{user.Address} 821 :User unaway";
        public static string IRCX_RPL_USERNOWAWAY_822(Server server, User user) => $":{user.Address} 822 :%s";

        public static string IRCX_RPL_REVEAL_851(Server server, User user) => $":{server} 851 {user} %s %s %s %s :%s";
        public static string IRCX_RPL_REVEALEND_852(Server server, User user) => $":{server} 852 {user} :End of ListX";

        public static string IRCX_ERR_BADCOMMAND_900(Server server, User user) => $":{server} 900 {user} %s :Bad command";
        public static string IRCX_ERR_TOOMANYARGUMENTS_901(Server server, User user) => $":{server} 901 {user} %s :Too many arguments";
        public static string IRCX_ERR_BADLYFORMEDPARAMS_902(Server server, User user) => $":{server} 902 {user} :Badly formed parameters";
        public static string IRCX_ERR_BADLEVEL_903(Server server, User user) => $":{server} 903 {user} %s :Bad level";
        public static string IRCX_ERR_BADPROPERTY_905(Server server, User user) => $":{server} 905 {user} %s :Bad property specified";
        public static string IRCX_ERR_BADVALUE_906(Server server, User user) => $":{server} 906 {user} %s :Bad value specified";
        public static string IRCX_ERR_RESOURCE_907(Server server, User user) => $":{server} 907 {user} :Not enough resources";
        public static string IRCX_ERR_SECURITY_908(Server server, User user) => $":{server} 908 {user} :No permissions to perform command";
        public static string IRCX_ERR_ALREADYAUTHENTICATED_909(Server server, User user) => $":{server} 909 {user} %s :Already authenticated";
        public static string IRCX_ERR_AUTHENTICATIONFAILED_910(Server server, User user) => $":{server} 910 {user} %s :Authentication failed";
        public static string IRCX_ERR_AUTHENTICATIONSUSPENDED_911(Server server, User user) => $":{server} 911 {user} %s :Authentication suspended for this IP";
        public static string IRCX_ERR_UNKNOWNPACKAGE_912(Server server, User user) => $":{server} 912 {user} %s :Unsupported authentication package";
        public static string IRCX_ERR_NOACCESS_913(Server server, User user) => $":{server} 913 {user} %s :No access";
        public static string IRCX_ERR_DUPACCESS_914(Server server, User user) => $":{server} 914 {user} :Duplicate access entry";
        public static string IRCX_ERR_MISACCESS_915(Server server, User user) => $":{server} 915 {user} :Unknown access entry";
        public static string IRCX_ERR_TOOMANYACCESSES_916(Server server, User user) => $":{server} 916 {user} :Too many access entries";
        public static string IRCX_ERR_EVENTDUP_918(Server server, User user) => $":{server} 918 {user} %s %s :Duplicate event entry";
        public static string IRCX_ERR_EVENTMIS_919(Server server, User user) => $":{server} 919 {user} %s %s :Unknown event entry";
        public static string IRCX_ERR_NOSUCHEVENT_920(Server server, User user) => $":{server} 920 {user} %s :No such event";
        public static string IRCX_ERR_TOOMANYEVENTS_921(Server server, User user) => $":{server} 921 {user} %s :Too many events specified";
        public static string IRCX_ERR_ACCESSNOTCLEAR_922(Server server, User user) => $":{server} 922 {user} :Some entries not cleared due to security";
        public static string IRCX_ERR_NOWHISPER_923(Server server, User user, Channel channel) => $":{server} 923 {user} {channel} :Does not permit whispers";
        public static string IRCX_ERR_NOSUCHOBJECT_924(Server server, User user) => $":{server} 924 {user} %s :No such object found";
        public static string IRCX_ERR_ALREADYONCHANNEL_927(Server server, User user) => $":{server} 927 {user} %s :Already in the channel.";
        public static string IRCX_ERR_ALREADYONCHANNEL_927X(Server server, User user, Channel channel) => $":{server} 927 {user} {channel} :Already in the channel.";
        public static string IRCX_ERR_U_NOTINCHANNEL_928(Server server, User user) => $"{server} 928 {user} :You're not in a channel";
        public static string IRCX_ERR_TOOMANYINVITES_929(Server server, User user) => $":{server} 929 {user} %s %s :Cannot invite. Too many invites.";
        public static string IRCX_ERR_NOTIMPLEMENTED(Server server, User user) => $":{server} 999 {user} :%s Sorry, this command is not implemented yet.";
        public static string IRCX_ERR_EXCEPTION(Server server, User user) => $":{server} 999 {user} :%s Oops! Looks like you've hit a snag here, please can you kindly report this.";
    }
}
