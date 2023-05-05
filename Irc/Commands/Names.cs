using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Modes.Channel;
using Irc.Objects;

namespace Irc.Commands;

internal class Names : Command, ICommand
{
    public Names() : base(1) { }
    public new EnumCommandDataType GetDataType() => EnumCommandDataType.None;

    public new void Execute(ChatFrame chatFrame)
    {
        var channelName = chatFrame.Message.Parameters.First();

        var channel = chatFrame.Server.GetChannelByName(channelName);

        if (channel != null)
            ProcessNamesReply(chatFrame.User, channel);
        else
            chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
    }

    public static void ProcessNamesReply(IUser user, IChannel channel)
    {
        // RFC 2812 "=" for others(public channels).
        char channelType = '=';

        if (channel.Modes.Secret)
        {
            // RFC 2812 "@" is used for secret channels
            channelType = '@';
        }
        else if (channel.Modes.Private)
        {
            // RFC 2812 "*" for private
            channelType = '*';
        }

        user.Send(
            Raw.IRCX_RPL_NAMEREPLY_353(user.Server, user, channel, channelType,
                                        string.Join(' ', 
                                                    channel.GetMembers().Select(m => 
                                                        $"{user.GetProtocol().FormattedUser(m)}"
                                                    )
                                              )
                                        )
            );
        user.Send(Raw.IRCX_RPL_ENDOFNAMES_366(user.Server, user, channel));
    }
}