using Irc.Enumerations;
using Irc.Modes;
using Irc.Objects;

namespace Irc.Commands;

public class Pass : Command, ICommand
{
    public Pass() : base(1, false) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        if (!chatFrame.User.IsRegistered())
        {
            // TODO: Encrypt below pass
            chatFrame.User.GetDataStore().Set("pass", chatFrame.Message.Parameters.First());
        }
        else
        {
            chatFrame.User.Send(Constants.IrcRaws.IRC_RAW_462(chatFrame.Server, chatFrame.User));
        }
    }
}