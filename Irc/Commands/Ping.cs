using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

internal class Ping : Command, ICommand
{
    public Ping() : base(1, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send($"PONG :{chatFrame.Message.Parameters.First()}");
    }
}