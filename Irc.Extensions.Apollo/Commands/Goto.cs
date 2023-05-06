using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions;

namespace Irc.Commands;

public class Goto : Command, ICommand
{
    public Goto() : base(2) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var channelName = chatFrame.Message.Parameters.First();
        var targetNickname = chatFrame.Message.Parameters[1];

        var channel = server.GetChannelByName(channelName);
        if (channel == null)
        {
            // No such channel
            user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, channelName));
            return;
        }

        var member = channel.GetMemberByNickname(targetNickname);
        if (member == null)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHNICK_401(server, user, targetNickname));
            return;
        }

        EnumChannelAccessResult channelAccessResult = channel.GetAccess(user, null, true);

        if (!channel.Allows(user) || channelAccessResult < EnumChannelAccessResult.SUCCESS_GUEST)
        {
            Join.SendJoinError(server, channel, user, channelAccessResult);
            return;
        }

        channel.Join(user, channelAccessResult)
        .SendTopic(user)
        .SendNames(user);
    }
}