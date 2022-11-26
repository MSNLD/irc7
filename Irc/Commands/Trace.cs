using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Commands;

internal class Trace : Command, ICommand
{
    public Trace() : base(0) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_COMMANDUNSUPPORTED_554(chatFrame.Server, chatFrame.User, nameof(Trace)));
    }
}