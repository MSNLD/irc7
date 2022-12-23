using Irc.Commands;
using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Commands;

internal class Away : Command, ICommand
{
    public Away(): base(0, true)
    {
        
    }
    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        if (chatFrame.Message.Parameters.Count == 0)
        {
            // Here
            chatFrame.User.Send(Raw.IRCX_RPL_UNAWAY_305(chatFrame.Server, chatFrame.User));
            chatFrame.User.BroadcastToChannels(Raw.IRCX_RPL_USERUNAWAY_821(chatFrame.Server, chatFrame.User), true);
            return;
        }

        // Gone
        var reason = chatFrame.Message.Parameters.First();
        chatFrame.User.Send(Raw.IRCX_RPL_NOWAWAY_306(chatFrame.Server, chatFrame.User));
        chatFrame.User.BroadcastToChannels(Raw.IRCX_RPL_USERNOWAWAY_822(chatFrame.Server, chatFrame.User, reason), true);
    }
}