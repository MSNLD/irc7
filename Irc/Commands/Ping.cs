using Irc.Enumerations;

namespace Irc.Commands;

internal class Ping : Command, ICommand
{
    public Ping()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send($"PONG :{chatFrame.Message.Parameters.First()}");
    }
}