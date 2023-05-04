using System;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;

namespace Irc.Modes
{
	public class ModeRuleChannel: ModeRule, IModeRule
	{
        private readonly EnumChannelAccessLevel accessLevel;

        public ModeRuleChannel(char modeChar, bool requiresParameter = false, int initialValue = 0, EnumChannelAccessLevel accessLevel = EnumChannelAccessLevel.ChatHost):
            base(modeChar, requiresParameter, initialValue)
		{
            this.accessLevel = accessLevel;
        }

        public EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            var user = (IUser)source;
            var channel = (IChannel)target;
            var member = channel.GetMember(user);

            if (member == null && !user.IsAdministrator())
            {
                return EnumIrcError.ERR_NOTONCHANNEL;
            }

            if (member.GetLevel() < accessLevel)
            {
                return EnumIrcError.ERR_NOCHANOP;
            }

            return EnumIrcError.OK;
        }
    }
}

