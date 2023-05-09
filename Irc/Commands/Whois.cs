using System.Linq;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Commands;

public class Whois : Command, ICommand
{
    public Whois() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        /*
         <- :sky-8a15b323126 311 Sky Sky ~no 192.168.88.131 * :Sky
         <- :sky-8a15b323126 319 Sky Sky :.#test
         <- :sky-8a15b323126 312 Sky Sky sky-8a15b323126 :Microsoft Exchange Chat Service
         <- :sky-8a15b323126 318 Sky Sky :End of /WHOIS list
        */
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var nicknameString = chatFrame.Message.Parameters.First();
        var nicknames = nicknameString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var nickname in nicknames)
        {
            ProcessWhoisReply(chatFrame.Server, chatFrame.User, nickname);
        }
        user.Send(IrcRaws.IRC_RAW_318(server, user, nicknameString));
    }

    public static void ProcessWhoisReply(IServer server, IUser user, string nickname)
    {
        IUser targetUser = server.GetUserByNickname(nickname);

        if (targetUser == null)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHNICK_401(server, user, nickname));
            return;
        }

        user.Send(IrcRaws.IRC_RAW_311(server, user, targetUser));

        if (targetUser.GetChannels().Count > 0)
        {
            var channels = targetUser.GetChannels();
            var channelStrings = channels.Select(c => $"{c.Value.GetListedMode()}{c.Key}").ToArray<string>();

            // TODO: Properly format channels & user modes
            user.Send(IrcRaws.IRC_RAW_319(server, user, targetUser,
                string.Join(' ', channelStrings)
            ));
        }

        if (targetUser.GetLevel() >= EnumUserAccessLevel.Guide)
        {
            user.Send(IrcRaws.IRC_RAW_313(server, user, targetUser));
        }

        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var secondsSinceLogin = (targetUser.LoggedOn - epoch).Ticks / TimeSpan.TicksPerSecond;
        var secondsIdle = (DateTime.UtcNow.Ticks - targetUser.LastIdle.Ticks) / TimeSpan.TicksPerSecond;

        user.Send(IrcRaws.IRC_RAW_317(server, user, targetUser, secondsIdle, secondsSinceLogin));
    }
}