using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Props.Channel
{
    internal class Topic : PropRule
    {
        public Topic() : base(ExtendedResources.ChannelPropTopic, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatHost, string.Empty, false)
        {

        }
    }
}
