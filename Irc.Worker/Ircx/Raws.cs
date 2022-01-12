using System.Text;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker.Ircx;

internal class Raws
{
    public static string IRCX_CLOSINGLINK = "ERROR :Closing Link: %n[%s] (%s)";

    public static string IRCX_CLOSINGLINK_007_SYSTEMKILL =
        "ERROR :Closing Link: %n[%s] 007 (Killed by system operator)";

    public static string IRCX_CLOSINGLINK_008_INPUTFLOODING = "ERROR :Closing Link: %n[%s] 008 (Input flooding)";
    public static string IRCX_CLOSINGLINK_009_OUTPUTSATURATION = "ERROR :Closing Link: %n[%s] 009 (Output Saturation)";
    public static string IRCX_CLOSINGLINK_011_PINGTIMEOUT = "ERROR :Closing Link: %n[%s] 011 (Ping timeout)";

    public static string RPL_AUTH_INIT = "AUTH %s I :%s";
    public static string RPL_AUTH_SEC_REPLY = "AUTH %s S :%s";
    public static string RPL_AUTH_SUCCESS = "AUTH %s * %s %o";
    public static string RPL_JOIN_IRC = ":%u JOIN :%c";
    public static string RPL_JOIN_MSN = ":%u JOIN %S :%c";
    public static string RPL_PART_IRC = ":%u PART %c";
    public static string RPL_QUIT_IRC = ":%u QUIT :%s";
    public static string RPL_MODE_IRC = ":%u MODE %s %s";
    public static string RPL_TOPIC_IRC = ":%u TOPIC %c :%s";
    public static string RPL_PROP_IRCX = ":%u PROP %c %s :%s";
    public static string RPL_KICK_IRC = ":%u KICK %c %s :%s";
    public static string RPL_KILL_IRC = ":%u KILL %s :%s";
    public static string RPL_SERVERKILL_IRC = ":%h KILL %s :%s";
    public static string RPL_KILLED = ":%s KILLED";
    public static string RPL_MSG_CHAN = ":%u %s %c :%s";
    public static string RPL_MSG_USER = ":%u %s %s :%s";
    public static string RPL_CHAN_MSG = ":%s %s %s :%s";
    public static string RPL_CHAN_WHISPER = ":%u WHISPER %c %s :%s";
    public static string RPL_EPRIVMSG_CHAN = ":%u EPRIVMSG %c :%s";
    public static string RPL_NOTICE_USER = ":%u NOTICE %s :%s";
    public static string RPL_PRIVMSG_USER = ":%u PRIVMSG %s :%s";
    public static string RPL_NOTICE_CHAN = ":%u NOTICE %c :%s";
    public static string RPL_PRIVMSG_CHAN = ":%u PRIVMSG %c :%s";
    public static string RPL_INVITE = ":%u INVITE %s %s :%c";
    public static string RPL_KNOCK_CHAN = ":%u KNOCK %c %s";
    public static string RPL_NICK = ":%u NICK %s";
    public static string RPL_PONG = "PONG %h :%n";
    public static string RPL_PONG_CLIENT = "PONG :%s";
    public static string RPL_PING = "PING :%h";
    public static string RPL_SERVICE_DATA = ":%h SERVICE %s %s"; //e.g. :SERVERNAMEs SERVICE DELETE CHANNEL %#Channel


    public static string IRCX_RPL_WELCOME_001 = ":%h 001 %n :Welcome to the %s server %n";
    public static string IRCX_RPL_WELCOME_002 = ":%h 002 %n :Your host is %h, running version %d.%d.%d";
    public static string IRCX_RPL_WELCOME_003 = ":%h 003 %n :This server was created %s";
    public static string IRCX_RPL_WELCOME_004 = ":%h 004 %n %h %d.%d.%d aioxz abcdefhiklmnoprstuvxyz";
    public static string IRCX_RPL_WELCOME_005 = ":%h 005 %n IRCX PREFIX=(qov).@+ CHANTYPES=%# CHANLIMIT=%#:1";

    public static string IRCX_RPL_UMODEIS_221 = ":%h 221 %n %s";

    public static string IRCX_RPL_LUSERCLIENT_251 = ":%h 251 %n :There are %d users and %d invisible on %d servers";
    public static string IRCX_RPL_LUSEROP_252 = ":%h 252 %n %d :operator(s) online";
    public static string IRCX_RPL_LUSERUNKNOWN_253 = ":%h 253 %n %d :unknown connection(s)";
    public static string IRCX_RPL_LUSERCHANNELS_254 = ":%h 254 %n %d :channels formed";
    public static string IRCX_RPL_LUSERME_255 = ":%h 255 %n :I have %d clients and %d servers";

    public static string IRCX_RPL_ADMINME_256 = ":%h 256 %n :Administrative info about %h";
    public static string IRCX_RPL_ADMINLOC1_257 = ":%h 257 %n :%s";
    public static string IRCX_RPL_ADMINLOC1_258 = ":%h 258 %n :%s";
    public static string IRCX_RPL_ADMINEMAIL_259 = ":%h 259 %n :%s";

    public static string IRCX_RPL_LUSERS_265 = ":%h 265 %n :Current local users: %d Max: %d";
    public static string IRCX_RPL_GUSERS_266 = ":%h 266 %n :Current global users: %d Max: %d";

    public static string IRCX_RPL_AWAY_301 = ":%h 301 %n %s :%s";
    public static string IRCX_RPL_USERHOST_302 = ":%h 302 %n :%s";
    public static string IRCX_RPL_UNAWAY_305 = ":%h 305 %n :You are no longer marked as being away";
    public static string IRCX_RPL_NOWAWAY_306 = ":%h 306 %n :You have been marked as being away";
    public static string IRCX_RPL_WHOISUSER_311 = ":%h 311 %n %s %s %s * :%s";
    public static string IRCX_RPL_WHOISSERVER_312 = ":%h 312 %n %s %s :%s";
    public static string IRCX_RPL_WHOISOPERATOR_313A = ":%h 313 %n %s :is an IRC administrator";
    public static string IRCX_RPL_WHOISOPERATOR_313O = ":%h 313 %n %s :is an IRC operator";
    public static string IRCX_RPL_ENDOFWHO_315 = ":%h 315 %n %s :End of /WHO list";
    public static string IRCX_RPL_WHOISIDLE_317 = ":%h 317 %n %s %d %d :seconds idle, signon time";
    public static string IRCX_RPL_ENDOFWHOIS_318 = ":%h 318 %n %s :End of /WHOIS list";
    public static string IRCX_RPL_WHOISCHANNELS_319 = ":%h 319 %n %s :%s";
    public static string IRCX_RPL_WHOISCHANNELS_319X = ":%h 319 %n %s :";
    public static string IRCX_RPL_WHOISIP_320 = ":%h 320 %n %s :from IP %s";
    public static string IRCX_RPL_MODE_324 = ":%h 324 %n %c %s";
    public static string IRCX_RPL_NOTOPIC_331 = ":%h 331 %n %c :No topic is set";
    public static string IRCX_RPL_TOPIC_332 = ":%h 332 %n %c :%s";
    public static string IRCX_RPL_VERSION_351 = ":%h 351 %n %d.%d.%d %h :%h %d.%d";
    public static string IRCX_RPL_WHOREPLY_352 = ":%h 352 %n %s %s %s %h %s %s%s%s :0 %s";
    public static string IRCX_RPL_NAMEREPLY_353 = ":%h 353 %n = %c :%s";
    public static string IRCX_RPL_NAMEREPLY_353X = ":%h 353 %n = %c :";
    public static string IRCX_RPL_ENDOFNAMES_366 = ":%h 366 %n %s :End of /NAMES list.";
    public static string IRCX_RPL_BANLIST_367 = ":%h 367 %n %c %s";
    public static string IRCX_RPL_ENDOFBANLIST_368 = ":%h 368 %n %c :End of Channel Ban List";
    public static string IRCX_RPL_RPL_INFO_371 = ":%h 371 %n :On-line since %s";
    public static string IRCX_RPL_RPL_INFO_371_VERS = ":%h 371 %n :%s %d.%d";
    public static string IRCX_RPL_RPL_MOTD_372 = ":%h 372 %n :- %s";
    public static string IRCX_RPL_RPL_ENDOFINFO_374 = ":%h 374 %n :End of /INFO list";
    public static string IRCX_RPL_RPL_MOTDSTART_375 = ":%h 375 %n :- %h Message of the Day -";
    public static string IRCX_RPL_RPL_ENDOFMOTD_376 = ":%h 376 %n :End of /MOTD command";

    public static string IRCX_RPL_YOUREOPER_381 = ":%h 381 %n :You are now an IRC operator";
    public static string IRCX_RPL_YOUREADMIN_386 = ":%h 386 %n :You are now an IRC administrator";
    public static string IRCX_RPL_TIME_391 = ":%h 391 %n %h :%S";

    public static string IRCX_ERR_NOSUCHNICK_401_N = ":%h 401 %n %s :No such nick";
    public static string IRCX_ERR_NOSUCHNICK_401 = ":%h 401 %n %s :No such nick/channel";
    public static string IRCX_ERR_NOSUCHCHANNEL_403 = ":%h 403 %n %s :No such channel";
    public static string IRCX_ERR_CANNOTSENDTOCHAN_404 = ":%h 404 %n %s :Cannot send to channel";
    public static string IRCX_ERR_CANNOTSENDTOCHAN_404X = ":%h 404 %n %c :Cannot send to channel";
    public static string IRCX_ERR_TOOMANYCHANNELS_405 = ":%h 405 %n %c :You have joined too many channels";
    public static string IRCX_ERR_NOORIGIN_409 = ":%h 409 %n :No origin specified";
    public static string IRC_ERR_NORECIPIENT_411 = ":%h 411 %n :No recipient given (%s)";
    public static string IRC_ERR_NOTEXT_412 = ":%h 412 %n :No text to send (%s)";
    public static string IRCX_ERR_UNKNOWNCOMMAND_421 = ":%h 421 %n %s :Unknown command";
    public static string IRCX_ERR_UNKNOWNCOMMAND_421_T = ":%h 421 %n %s :String parameter must be 160 chars or less";
    public static string IRCX_ERR_NOMOTD_422 = ":%h 422 %n :MOTD File is missing";
    public static string IRCX_ERR_ERRONEOUSNICK_432 = ":%h 432 %s :Erroneous nickname";
    public static string IRCX_ERR_NICKINUSE_433 = ":%h 433 * %s :Nickname is already in use";
    public static string IRCX_ERR_NONICKCHANGES_439 = ":%h 439 %n %s :Nick name changes not permitted.";
    public static string IRCX_ERR_NOTONCHANNEL_442 = ":%h 442 %n %s :You're not on that channel";
    public static string IRCX_ERR_USERONCHANNEL_443 = ":%h 443 %n %s %s :is already on channel";
    public static string IRCX_ERR_NOTREGISTERED_451 = ":%h 451 %n :You have not registered";
    public static string IRCX_ERR_NEEDMOREPARAMS_461 = ":%h 461 %n %s :Not enough parameters";
    public static string IRCX_ERR_ALREADYREGISTERED_462 = ":%h 462 %n :You may not reregister";
    public static string IRCX_ERR_KEYSET_467 = ":%h 467 %n %c :Channel key already set";
    public static string IRCX_ERR_CHANNELISFULL_471 = ":%h 471 %n %c :Cannot join channel (+l)";
    public static string IRCX_ERR_UNKNOWNMODE_472 = ":%h 472 %n %l :is unknown mode char to me";
    public static string IRCX_ERR_INVITEONLYCHAN_473 = ":%h 473 %n %c :Cannot join channel (+i)";
    public static string IRCX_ERR_BANNEDFROMCHAN_474 = ":%h 474 %n %c :Cannot join channel (+b)";
    public static string IRCX_ERR_BADCHANNELKEY_475 = ":%h 475 %n %c :Cannot join channel (+k)";
    public static string IRCX_ERR_NOPRIVILEGES_481 = ":%h 481 %n :Permission Denied - You're not an IRC operator";
    public static string IRCX_ERR_CHANOPRIVSNEEDED_482 = ":%h 482 %n %c :You're not channel operator";
    public static string IRCX_ERR_CHANQPRIVSNEEDED_485 = ":%h 485 %n %c :You're not channel owner";

    public static string IRCX_ERR_USERSDONTMATCH_502 = ":%h 502 %n :Cant change mode for other users";

    public static string IRCX_ERR_OPTIONUNSUPPORTED_555 =
        ":%h 555 %n %s :Server option for this command is not supported.";

    public static string IRCX_ERR_AUTHONLYCHAN_556 = ":%h 556 %n %c :Only authenticated users may join channel.";
    public static string IRCX_ERR_SECUREONLYCHAN_557 = ":%h 557 %n %c :Only secure users may join channel.";

    public static string IRCX_RPL_FINDS_613 = ":%h 613 %n :%s %s";
    public static string IRCX_RPL_LISTRSTART_614 = ":%h 811 %n :Start of ListR";
    public static string IRCX_RPL_LISTRLIST_614 = ":%h 812 %n %c %d %s :%s";
    public static string IRCX_RPL_LISTREND_614 = ":%h 817 %n :End of ListR";
    public static string IRCX_RPL_YOUREGUIDE_629 = ":%h 629 %n :You are now an IRC guide";

    public static string IRC2_RPL_WHOISSECURE_671 = ":%h 671 %n %s :is using a secure connection";

    public static string IRCX_RPL_FINDS_NOSUCHCAT_701 = ":%h 701 %n :Category not found";
    public static string IRCX_RPL_FINDS_NOTFOUND_702 = ":%h 702 %n :Channel not found";
    public static string IRCX_RPL_FINDS_DOWN_703 = ":%h 703 %n :Server down. Retry later.";
    public static string IRCX_RPL_FINDS_CHANNELEXISTS_705 = ":%h 705 %n :Channel with same name exists";
    public static string IRCX_RPL_FINDS_INVALIDCHANNEL_706 = ":%h 706 %n :Channel name is not valid";

    public static string IRCX_RPL_IRCX_800 = ":%h 800 %n %d %d %s %d %s";
    public static string IRCX_RPL_ACCESSADD_801 = ":%h 801 %n %s %s %s %d %s :%s";
    public static string IRCX_RPL_ACCESSDELETE_802 = ":%h 802 %n %s %s %s %d %s :%s";
    public static string IRCX_RPL_ACCESSSTART_803 = ":%h 803 %n %s :Start of access entries";
    public static string IRCX_RPL_ACCESSLIST_804 = ":%h 804 %n %s %s %s %d %s :%s";
    public static string IRCX_RPL_ACCESSEND_805 = ":%h 805 %n %s :End of access entries";
    public static string IRCX_RPL_EVENTADD_806 = ":%h 806 %n %s %s";
    public static string IRCX_RPL_EVENTDEL_807 = ":%h 807 %n %s %s";
    public static string IRCX_RPL_EVENTSTART_808 = ":%h 808 %n %s :Start of events";
    public static string IRCX_RPL_EVENTLIST_809 = ":%h 809 %n %s %s";
    public static string IRCX_RPL_EVENTEND_810 = ":%h 810 %n %s :End of events";
    public static string IRCX_RPL_LISTXSTART_811 = ":%h 811 %n :Start of ListX";
    public static string IRCX_RPL_LISTXLIST_812 = ":%h 812 %n %s %s %d %d :%s";
    public static string IRCX_RPL_LISTXPICS_813 = ":%h 813 %n :%s";
    public static string IRCX_RPL_LISTXTRUNC_816 = ":%h 816 %n :Truncation of ListX";
    public static string IRCX_RPL_LISTXEND_817 = ":%h 817 %n :End of ListX";

    public static string IRCX_RPL_PROPLIST_818 = ":%h 818 %n %s %s :%s";
    public static string IRCX_RPL_PROPEND_819 = ":%h 819 %n %s :End of properties";
    public static string IRCX_RPL_ACCESSCLEAR_820 = ":%h 820 %n %s %s :Clear";
    public static string IRCX_RPL_USERUNAWAY_821 = ":%u 821 :User unaway";
    public static string IRCX_RPL_USERNOWAWAY_822 = ":%u 822 :%s";

    public static string IRCX_RPL_REVEAL_851 = ":%h 851 %n %s %s %s %s :%s";
    public static string IRCX_RPL_REVEALEND_852 = ":%h 852 %n :End of ListX";

    public static string IRCX_ERR_BADCOMMAND_900 = ":%h 900 %n %s :Bad command";
    public static string IRCX_ERR_TOOMANYARGUMENTS_901 = ":%h 901 %n %s :Too many arguments";
    public static string IRCX_ERR_BADLYFORMEDPARAMS_902 = ":%h 902 %n :Badly formed parameters";
    public static string IRCX_ERR_BADLEVEL_903 = ":%h 903 %n %s :Bad level";
    public static string IRCX_ERR_BADPROPERTY_905 = ":%h 905 %n %s :Bad property specified";
    public static string IRCX_ERR_BADVALUE_906 = ":%h 906 %n %s :Bad value specified";
    public static string IRCX_ERR_RESOURCE_907 = ":%h 907 %n :Not enough resources";
    public static string IRCX_ERR_SECURITY_908 = ":%h 908 %n :No permissions to perform command";
    public static string IRCX_ERR_ALREADYAUTHENTICATED_909 = ":%h 909 %n %s :Already authenticated";
    public static string IRCX_ERR_AUTHENTICATIONFAILED_910 = ":%h 910 %n %s :Authentication failed";
    public static string IRCX_ERR_AUTHENTICATIONSUSPENDED_911 = ":%h 911 %n %s :Authentication suspended for this IP";
    public static string IRCX_ERR_UNKNOWNPACKAGE_912 = ":%h 912 %n %s :Unsupported authentication package";
    public static string IRCX_ERR_NOACCESS_913 = ":%h 913 %n %s :No access";
    public static string IRCX_ERR_DUPACCESS_914 = ":%h 914 %n :Duplicate access entry";
    public static string IRCX_ERR_MISACCESS_915 = ":%h 915 %n :Unknown access entry";
    public static string IRCX_ERR_TOOMANYACCESSES_916 = ":%h 916 %n :Too many access entries";
    public static string IRCX_ERR_EVENTDUP_918 = ":%h 918 %n %s %s :Duplicate event entry";
    public static string IRCX_ERR_EVENTMIS_919 = ":%h 919 %n %s %s :Unknown event entry";
    public static string IRCX_ERR_NOSUCHEVENT_920 = ":%h 920 %n %s :No such event";
    public static string IRCX_ERR_TOOMANYEVENTS_921 = ":%h 921 %n %s :Too many events specified";
    public static string IRCX_ERR_ACCESSNOTCLEAR_922 = ":%h 922 %n :Some entries not cleared due to security";
    public static string IRCX_ERR_NOWHISPER_923 = ":%h 923 %n %c :Does not permit whispers";
    public static string IRCX_ERR_NOSUCHOBJECT_924 = ":%h 924 %n %s :No such object found";
    public static string IRCX_ERR_ALREADYONCHANNEL_927 = ":%h 927 %n %s :Already in the channel.";
    public static string IRCX_ERR_ALREADYONCHANNEL_927X = ":%h 927 %n %c :Already in the channel.";
    public static string IRCX_ERR_U_NOTINCHANNEL_928 = "%h 928 %n :You're not in a channel";
    public static string IRCX_ERR_TOOMANYINVITES_929 = ":%h 929 %n %s %s :Cannot invite. Too many invites.";
    public static string IRCX_ERR_NOTIMPLEMENTED = ":%h 999 %n :%s Sorry, this command is not implemented yet.";

    public static string IRCX_ERR_EXCEPTION =
        ":%h 999 %n :%s Oops! Looks like you've hit a snag here, please can you kindly report this.";


    public static string Create(Server Server = null, Channel Channel = null, Client Client = null, string Raw = null,
        string[] Data = null, int[] IData = null, bool Newline = true)
    {
        int dataLen = 0, offsetd = 0, offseti = 0, remainder = 0;

        var output = new StringBuilder(512);
        remainder = 510 - Raw.Length;

        for (var i = 0; i < Raw.Length; i++)
            switch (Raw[i])
            {
                case '%':
                {
                    i++;
                    if (i < Raw.Length)
                        switch (Raw[i])
                        {
                            case 's':
                            {
                                var len = Data[offsetd].Length;
                                if (len > 0)
                                {
                                    if (len > remainder) len = remainder;
                                    output.Append(Data[offsetd++].Substring(0, len));
                                    remainder -= len;
                                }

                                break;
                            }
                            case 'S':
                            {
                                //put all of string array in
                                while (offsetd < Data.Length)
                                {
                                    output.Append(Data[offsetd]);
                                    remainder -= Data[offsetd++].Length;
                                }

                                break;
                            }
                            case 'd':
                            {
                                output.Append(IData[offseti].ToString());
                                remainder -= IData[offseti++].ToString().Length;
                                break;
                            }
                            case 'x':
                            {
                                var value = IData[offseti++].ToString("X");
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'o':
                            {
                                var value = IData[offseti++].ToString("X9");
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'l':
                            {
                                remainder -= 1;
                                output.Append((char) IData[offseti++]);
                                break;
                            }
                            case 'h':
                            {
                                var value = Server.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'n':
                            {
                                var value = Client.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            //case (byte)'a': { output += User.Nickname(); break; }
                            //case (byte)'i': { string sValue = (string)parameters[0]; output += sValue; parameters.RemoveAt(0); break; }
                            case 'c':
                            {
                                var value = Channel.Name;
                                remainder -= value.Length;
                                output.Append(value);
                                break;
                            }
                            case 'u':
                            {
                                remainder -= Client.Address._address[2].Length;
                                output.Append(Client.Address._address[2]);
                                break;
                            }
                            default:
                            {
                                output.Append('%');
                                output.Append(Raw[i]);
                                remainder -= 2;
                                break;
                            }
                        }

                    break;
                }
                default:
                {
                    output.Append(Raw[i]);
                    remainder--;
                    break;
                }
            }

        if (Data != null) //Append overflow
            while (offsetd < Data.Length)
                output.Append(Data[offsetd++]);

        if (Newline) output.Append(Resources.CRLF);
        return new string(output.ToString());
    }
}