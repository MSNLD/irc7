using Irc.Constants;
using Irc.Enumerations;

namespace Irc.Commands;

public class Pass : Command, ICommand
{
    public Pass() : base(1, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(ChatFrame chatFrame)
    {
        if (!chatFrame.User.IsRegistered())
            // TODO: Encrypt below pass
            chatFrame.User.GetDataStore().Set("pass", chatFrame.Message.Parameters.First());
        else
            chatFrame.User.Send(IrcRaws.IRC_RAW_462(chatFrame.Server, chatFrame.User));
    }
}