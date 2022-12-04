using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Whowas : Command, ICommand
{
    public Whowas() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_COMMANDUNSUPPORTED_554(chatFrame.Server, chatFrame.User, nameof(Whowas)));
    }
}