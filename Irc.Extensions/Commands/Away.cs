﻿using Irc.Commands;
using Irc.Enumerations;

namespace Irc.Extensions.Commands;

internal class Away : Command, ICommand
{
    public Away() : base()
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(ChatFrame chatFrame)
    {
        chatFrame.User.Send(Raw.IRCX_ERR_NOTIMPLEMENTED(chatFrame.Server, chatFrame.User, nameof(Away)));
    }
}