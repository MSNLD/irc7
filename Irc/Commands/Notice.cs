﻿using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

internal class Notice : Command, ICommand
{
    public Notice() : base(2)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.Standard;
    }

    public new void Execute(IChatFrame chatFrame)
    {
        Privmsg.SendMessage(chatFrame, true);
    }
}