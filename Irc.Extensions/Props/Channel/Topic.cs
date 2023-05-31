using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Extensions.Props.Channel
{
    public class Topic : PropRule
    {
        public Topic() : base(ExtendedResources.ChannelPropTopic, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatHost, Resources.ChannelPropTopicRegex, string.Empty, false)
        {
        }

        public override string GetValue(IChatObject target)
        {
            return ((IChannel)target).ChannelStore.Get("topic");
        }

        public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
        {
            var result = base.EvaluateSet(source, target, propValue);
            if (result != EnumIrcError.OK) return result;

            IChannel channel = (IChannel)target;
            channel.ChannelStore.Set("topic", propValue);
            return result;
        }
    }
}
