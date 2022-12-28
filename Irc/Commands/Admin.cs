using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using ICommand = Irc.Interfaces.ICommand;

namespace Irc.Commands;

public class Admin : Command, ICommand
{

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        /*
         <- :sky-8a15b323126 256 Sky :Administrative info about sky-8a15b323126
         <- :sky-8a15b323126 257 Sky :This is a message about Administrator information
         <- :sky-8a15b323126 258 Sky :This is the second line about Admin
         <- :sky-8a15b323126 259 Sky :
        */
        var hasAdminInfo = false;
        var adminInfo1 = chatFrame.Server.GetDataStore().Get("admininfo1");
        var adminInfo2 = chatFrame.Server.GetDataStore().Get("admininfo2");
        var adminInfo3 = chatFrame.Server.GetDataStore().Get("admininfo3");

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