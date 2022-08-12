using Irc.Constants;
using Irc.Enumerations;
using Irc.Objects;

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
        IUser targetUser = chatFrame.User;

        chatFrame.User.Send(IrcRaws.IRC_RAW_311(chatFrame.Server, chatFrame.User, targetUser));

        if (chatFrame.User.GetChannels().Count > 0)
        {
            // TODO: Properly format channels & user modes
            chatFrame.User.Send(IrcRaws.IRC_RAW_319(chatFrame.Server, chatFrame.User, targetUser,
                string.Join(' ', chatFrame.User.GetChannels())
            ));
        }

        if (targetUser.GetLevel() >= EnumUserAccessLevel.Guide)
        {
            chatFrame.User.Send(IrcRaws.IRC_RAW_313(chatFrame.Server, chatFrame.User, targetUser));
        }

        var idleSeconds = 1;
        var idleEpoch = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        if (idleSeconds > 0)
        {
            chatFrame.User.Send(IrcRaws.IRC_RAW_317(chatFrame.Server, chatFrame.User, targetUser, idleSeconds, idleEpoch));
        }

        chatFrame.User.Send(IrcRaws.IRC_RAW_318(chatFrame.Server, chatFrame.User, targetUser));

    }
}