using Irc.Constants;
using Irc.Enumerations;

namespace Irc.Commands;

public class Admin : Command, ICommand
{
    public Admin() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        /*
         <- :sky-8a15b323126 256 Sky :Administrative info about sky-8a15b323126
         <- :sky-8a15b323126 257 Sky :This is a message about Administrator information
         <- :sky-8a15b323126 258 Sky :This is the second line about Admin
         <- :sky-8a15b323126 259 Sky :
        */
        bool hasAdminInfo = false;
        var adminInfo1 = "Your Admin is X";
        var adminInfo2 = "Y and Z";
        var adminInfo3 = "admin@aol.com";

        if (!string.IsNullOrWhiteSpace(adminInfo1))
        {
            chatFrame.User.Send(IrcRaws.IRC_RAW_256(chatFrame.Server, chatFrame.User));
            chatFrame.User.Send(IrcRaws.IRC_RAW_257(chatFrame.Server, chatFrame.User, adminInfo1));
            chatFrame.User.Send(IrcRaws.IRC_RAW_258(chatFrame.Server, chatFrame.User, adminInfo2));
            chatFrame.User.Send(IrcRaws.IRC_RAW_259(chatFrame.Server, chatFrame.User, adminInfo3));
        }
        else
        {
            // <- :sky-8a15b323126 423 Sky sky-8a15b323126 :No administrative info available
            chatFrame.User.Send(IrcRaws.IRC_RAW_423(chatFrame.Server, chatFrame.User));
        }
    }
}