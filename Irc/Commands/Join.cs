using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Commands
{
    internal class Join : Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Join()
        {
            _requiredMinimumParameters = 1;
        }

        public void Execute(ChatFrame chatFrame)
        {
            var channelNames = chatFrame.Message.Parameters.First().Split(',', StringSplitOptions.RemoveEmptyEntries);

            chatFrame
                .Server
                .GetChannels()
                .Where(c => channelNames.Contains(c.GetName()))
                .Where(c => c.Allows(chatFrame.User))
                .ToList()
                .ForEach(
                    channel => 
                        channel.Join(chatFrame.User)
                               .SendTopic(chatFrame.User)
                               .SendNames(chatFrame.User)
                        );
        }
    }
}