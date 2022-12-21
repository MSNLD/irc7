﻿using Irc.Interfaces;
using Irc.Models.Enumerations;

namespace Irc.Commands;

internal class Pong : Command, ICommand
{
    public Pong() : base(0, false)
    {
    }

    public new EnumCommandDataType GetDataType()
    {
        return EnumCommandDataType.None;
    }

    public new void Execute(IChatFrame chatFrame)
    {
    }
}