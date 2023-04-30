using Irc.Constants;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes.Channel.Member
{
    public class Voice : ModeRule, IModeRule
    {
        public Voice() : base(Resources.MemberModeVoice, true)
        {
        }
        public EnumIrcError Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            var channel = (IChannel)target;
            if (!channel.CanBeModifiedBy(source)) return EnumIrcError.ERR_NOTONCHANNEL;

            IChannelMember targetMember = channel.GetMemberByNickname(parameter);
            if (targetMember == null) return EnumIrcError.ERR_NOSUCHNICK;

            IChannelMember sourceMember = channel.GetMember((IUser)source);

            EnumIrcError result = sourceMember.CanModify(targetMember, EnumChannelAccessLevel.ChatVoice, false);
            if (result == EnumIrcError.OK)
            {
                targetMember.SetVoice(flag);
                DispatchModeChange(source, target, flag, targetMember.GetUser().ToString());
            }

            return result;
        }
    }
}
