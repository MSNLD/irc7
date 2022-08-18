using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class Topic : PropRule
    {
        public Topic() : base(ExtendedResources.ChannelPropTopic, EnumChannelAccessLevel.ChatGuest, EnumChannelAccessLevel.ChatHost, string.Empty, false)
        {

        }
    }
}
