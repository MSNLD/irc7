using Irc.Commands;
using Irc.Enumerations;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Commands;

internal class Away : Command, ICommand
{
    public Away() : base(0, true) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        if (chatFrame.Message.Parameters.Count == 0) {
            user.SetBack(server, chatFrame.User);
            return;
        }

        var reason = chatFrame.Message.Parameters.First();
        user.SetAway(server, chatFrame.User, reason);
    }
}
