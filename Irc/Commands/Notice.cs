using Irc.Enumerations;

namespace Irc.Commands;

internal class Notice : Command, ICommand
{
    public Notice() : base(2) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.Standard;

    public new void Execute(ChatFrame chatFrame)
    {
        Privmsg.SendMessage(chatFrame, true);
    }
}