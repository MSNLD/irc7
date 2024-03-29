﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Extensions.Props.Channel
{
    internal class Memberkey : PropRule
    {
        // The MEMBERKEY channel property is the keyword required to enter the channel. The MEMBERKEY property is limited to 31 characters. 
        // It may never be read.
        public Memberkey() : base(ExtendedResources.ChannelPropMemberkey, EnumChannelAccessLevel.None, EnumChannelAccessLevel.ChatHost, Resources.GenericProps, string.Empty, false)
        {

        }

        public override EnumIrcError EvaluateSet(IChatObject source, IChatObject target, string propValue)
        {
            // MEMBERKEY being set to a value sends a prop reply but no MODE reply
            // Mode +k is enforced server-side however.

            // MEMBERKEY being set to blank sends a prop reply but no MODE reply
            // Mode -k is enforced server-side however

            var result = base.EvaluateSet(source, target, propValue);

            if (result == EnumIrcError.OK)
            {
                IChannel channel = (IChannel)target;
                channel.Modes.Key = propValue;
            }

            return result;
        }
    }
}
