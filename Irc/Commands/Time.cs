using Irc.Enumerations;

namespace Irc.Commands;

public class Time : Command, ICommand
{
    public Time() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_TIME_391(chatFrame.Server, chatFrame.User));
    }
}