using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Worker.Ircx.Objects;

namespace Irc.Commands
{
    internal class Part: Command, ICommand
    {
        public EnumCommandDataType GetDataType() => EnumCommandDataType.None;

        public Part()
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
                .ToList()
                .ForEach(
                    channel =>
                        channel.Part(chatFrame.User)
                );
        }
    }
}
