using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel
{
    public class TopicOp : ModeRule, IModeRule
    {
        public TopicOp() : base(Resources.ChannelModeTopicOp)
        {
        }

        EnumModeResult Evaluate(ChatObject chatObject, string modeValue)
        {
            return EnumModeResult.OK;
        }
    }
}
