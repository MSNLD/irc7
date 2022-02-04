using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Commands
{
    internal class Names : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Names()
        {
            _requiredMinimumParameters = 1;
        }

        public void Execute(ChatFrame chatFrame)
        {
            var channelName = chatFrame.Message.Parameters.First();

            var channel = chatFrame.Server.GetChannelByName(channelName);

            if (channel != null)
            {
                ProcessNamesReply(chatFrame.User, channel);
            }
            else
            {
                // TODO: To make common function for this
                chatFrame.User.Send(Raw.IRCX_ERR_NOSUCHCHANNEL_403(chatFrame.Server, chatFrame.User, channelName));
            }
        }

        public static void ProcessNamesReply(User user, IChannel channel)
        {
            user.Send(Raw.IRCX_RPL_NAMEREPLY_353(user.Server, user, channel, string.Join(' ', channel.GetMembers().Select(m => m.GetUser()))));
        }
    }
}