using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Whowas : Command, ICommand
{
    public Whowas()
    {
        _requiredMinimumParameters = 0;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_COMMANDUNSUPPORTED_554(chatFrame.Server, chatFrame.User, nameof(Whowas)));
    }
}