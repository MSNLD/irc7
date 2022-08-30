﻿using Irc.Enumerations;
using Irc.Extensions.Apollo.Objects.Channel;
using Irc.Interfaces;
using Irc.Modes;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo.Modes.User
{
    public class Host : ModeRule, IModeRule
    {
        public Host() : base(ApolloResources.UserModeHost, true)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            if (target == source && flag)
            {
                IUser user = (IUser)source;
                var channel = (ApolloChannel)user.GetChannels().LastOrDefault().Key;
                var member = user.GetChannels().LastOrDefault().Value;
                if (channel.PropCollection.GetProp("OWNERKEY").GetValue() == parameter)
                {
                    member.SetOwner(true);
                    channel.Modes.GetMode('q').DispatchModeChange(source, channel, true, target.ToString());
                }
                else if (channel.PropCollection.GetProp("HOSTKEY").GetValue() == parameter)
                {
                    member.SetHost(true);
                    channel.Modes.GetMode('o').DispatchModeChange(source, channel, true, target.ToString());
                }
                return EnumIrcError.OK;
            }
            return EnumIrcError.ERR_UNKNOWNMODEFLAG;
        }
    }
}