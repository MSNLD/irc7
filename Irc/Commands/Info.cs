﻿using Irc.Enumerations;
using Irc.Interfaces;

namespace Irc.Commands;

internal class Info : Command, ICommand
{
    public Info() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_VERS(chatFrame.Server, chatFrame.User,
            chatFrame.Server.ServerVersion));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_RUNAS(chatFrame.Server, chatFrame.User));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_INFO_371_UPTIME(chatFrame.Server, chatFrame.User,
            chatFrame.Server.GetDataStore().GetAs<DateTime>("creation")));
        chatFrame.User.Send(Raw.IRCX_RPL_RPL_ENDOFINFO_374(chatFrame.Server, chatFrame.User));
    }
}