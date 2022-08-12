using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel.Member
{
    public class Operator : ModeRule, IModeRule
    {
        /*
         -> sky-8a15b323126 MODE #test +q Sky2k
        <- :sky-8a15b323126 482 Sky3k #test :You're not channel operator
        -> sky-8a15b323126 MODE #test +o Sky2k
        <- :sky-8a15b323126 482 Sky3k #test :You're not channel operator
        <- :Sky2k!~no@127.0.0.1 MODE #test +o Sky3k
        -> sky-8a15b323126 MODE #test +q Sky2k
        <- :sky-8a15b323126 485 Sky3k #test :You're not channel owner
        -> sky-8a15b323126 MODE #test +o Sky2k
        <- :sky-8a15b323126 485 Sky3k #test :You're not channel owner
         */
        public Operator() : base(Resources.UserModeOper, true)
        {
        }

        public EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            if (source is IUser && target is IChannel)
            {
                IChannel channel = (IChannel)target;
                IChannelMember sourceMember = channel.GetMember((IUser)source);
                IChannelMember targetMember = channel.GetMemberByNickname(parameter);

                if (targetMember == null)
                {
                    // No such nickname?
                    return EnumModeResult.NOSUCH;
                }
                else
                {
                    if (sourceMember.IsHost())
                    {
                        targetMember.SetHost(flag);
                        DispatchChannelModeChange(source, target, flag, parameter);
                        return EnumModeResult.OK;
                    }
                    else
                    {
                        return EnumModeResult.NOTOPER;
                    }
                }
            }
            else
            {
                // invalid targets
                return EnumModeResult.NOSUCH;
            }
        }
    }
}
