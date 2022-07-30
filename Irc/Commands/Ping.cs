using Irc.Enumerations;

namespace Irc.Commands;

internal class Ping : Command, ICommand
{
    public Ping() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send($"PONG :{chatFrame.Message.Parameters.First()}");
    }
}