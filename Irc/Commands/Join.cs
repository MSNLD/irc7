using Irc.Enumerations;
using Irc.Objects;
using Irc.Objects.Channel;
using Irc.Objects.Server;

namespace Irc.Commands;

internal class Join : Command, ICommand
{
    public Join()
    {
        _requiredMinimumParameters = 1;
    }

    public EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public void Execute(ChatFrame chatFrame)
    {
        var server = chatFrame.Server;
        var user = chatFrame.User;
        var parameters = chatFrame.Message.Parameters;

        var channelNames = ValidateChannels(server, user, parameters);
        if (channelNames.Count == 0) return;

        var key = parameters.Count > 1 ? parameters[1] : string.Empty;
        JoinChannels(server, user, channelNames);
    }

    public static List<string> ValidateChannels(IServer server, IUser user, List<string> parameters)
    {
        var channelNames = parameters.First().Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (channelNames.Count == 0)
        {
            user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, string.Empty));
        }
        else
        {
            var invalidChannelNames = channelNames.Where(c => !Channel.ValidName(c)).ToList();
            channelNames.RemoveAll(c => invalidChannelNames.Contains(c));

            // TODO: Could do better below for reporting invalid channel / empty channel
            if (invalidChannelNames.Count > 0)
                invalidChannelNames.ForEach(c => user.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(server, user, c)));
        }

        return channelNames;
    }

    public void JoinChannels(IServer server, IUser user, List<string> channelNames)
    {
        server
            .GetChannels()
            .Where(c => channelNames.Contains(c.GetName()))
            .Where(c => c.Allows(user))
            .ToList()
            .ForEach(
                channel =>
                    channel.Join(user)
                        .SendTopic(user)
                        .SendNames(user)
            );
    }
}