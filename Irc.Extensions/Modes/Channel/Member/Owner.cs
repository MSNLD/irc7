﻿using Irc.Constants;
using Irc.Interfaces;
using Irc.Models.Enumerations;
using Irc.Modes;
using Irc.Objects;
using Irc.Objects.Server;

namespace Irc.Extensions.Modes.Channel.Member;

public class Owner : ModeRule, IModeRule
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
    public Owner() : base(Resources.MemberModeOwner, true)
    {
    }

    public new EnumIrcError Evaluate(IChatObject source, IChatObject target, bool flag, string parameter)
    {
        var channel = (IChannel)target;

        // Allowed to modify channel (server OR user is on channel?)
        // Is allowed to modify user
        var allowedToModify = source is IServer || ((IUser)source).GetChannels().Keys.Contains(channel);
        if (!allowedToModify) return EnumIrcError.ERR_NOTONCHANNEL;

        var sourceMember = channel.GetMember((IUser)source);
        var targetMember = channel.GetMemberByNickname(parameter);

        if (targetMember == null)
            // No such nickname?
            return EnumIrcError.ERR_NOSUCHNICK;

        var result = sourceMember.CanModify(targetMember, EnumChannelAccessLevel.ChatOwner);
        if (result == EnumIrcError.OK)
        {
            if (flag && targetMember.IsHost())
            {
                targetMember.SetHost(false);
                DispatchModeChange(Resources.MemberModeHost, source, target, false, parameter);
            }

            targetMember.SetOwner(flag);
            DispatchModeChange(source, target, flag, parameter);
        }

        return result;
    }
}