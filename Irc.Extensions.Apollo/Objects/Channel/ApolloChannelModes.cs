using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Extensions.Objects.Channel;

namespace Irc.Extensions.Apollo.Objects.Channel
{
    public class ApolloChannelModes: ExtendedChannelModes
    {
        public ApolloChannelModes(): base()
        {
            modes.Add(ApolloResources.ChannelModeNoGuestWhisper, 0);
            modes.Add(ApolloResources.ChannelModeOnStage, 0);
            modes.Add(ApolloResources.ChannelModeSubscriber, 0);
        }
    }
}
