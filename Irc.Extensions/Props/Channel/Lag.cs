using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class Lag : PropRule
    {
        // The LAG channel property contains a numeric value between 0 to 2 seconds.
        // The server will add an artificial delay of that length between subsequent messages from the same member.
        // All messages to the channel are affected. 
        public Lag() : base(ExtendedResources.ChannelPropLag, EnumChannelAccessLevel.ChatHost, EnumChannelAccessLevel.ChatHost, Resources.ChannelPropLagRegex, string.Empty, false)
        {

        }
    }
}
