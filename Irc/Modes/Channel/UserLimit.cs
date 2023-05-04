using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Channel = Irc.Objects.Channel.Channel;

namespace Irc.Modes.Channel
{
    public class UserLimit : ModeRuleChannel, IModeRule
    {
        public UserLimit() : base(Resources.ChannelModeUserLimit, true)
        {
        }

        public new EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            var result = base.Evaluate(source, target, flag, parameter);
            if (result != EnumIrcError.OK)
            {
                return result;
            }

            var user = (IUser)source;
            var channel = ((IChannel)target);
            var isAdministrator = user.IsAdministrator();

            if (flag == false)
            {
                if (isAdministrator)
                {
                    // TODO: Currently does not support unsetting limit without extra parameter

                    channel.GetModes().GetMode(Resources.ChannelModeUserLimit).Set(0);
                    DispatchModeChange(source, target, false, string.Empty);
                }
                return EnumIrcError.OK;
            }



            if (!int.TryParse(parameter, out int limit))
            {
                return EnumIrcError.ERR_NEEDMOREPARAMS;
            }

            if (limit > 0 && (limit <= 100 || isAdministrator))
            {
                channel.GetModes().GetMode(Resources.ChannelModeUserLimit).Set(limit);
                DispatchModeChange(source, target, true, limit.ToString());
            }
            return EnumIrcError.OK;
        }
    }
}
