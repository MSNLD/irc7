﻿using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects;
using Irc.Objects.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes
{
    public class ModeRule : IModeRule
    {
        protected char ModeChar { get; }
        private int ModeValue { get; set; }
        public bool RequiresParameter { get; }

        public ModeRule(char modeChar, bool requiresParameter = false, int initialValue = 0)
        {
            ModeChar = modeChar;
            ModeValue = initialValue;
            RequiresParameter = requiresParameter;
        }

        // Although the below is a string we are to evaluate and cast to integer
        // We can also throw bad value here if it is not the desired type
        public EnumModeResult Evaluate(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            throw new NotSupportedException();
        }

        public void DispatchChannelModeChange(ChatObject source, ChatObject target, bool flag, string parameter)
        {
            target.Send(
                Raw.RPL_MODE_IRC(
                        source,
                        target,
                        $"{(flag ? "+" : "-")}{ModeChar}{(parameter != null ? $" {parameter}" : string.Empty)}"
                    )
                );
        }

        public void Set(int value) => ModeValue = value;
        public int Get() => ModeValue;
        public char GetModeChar() => ModeChar;
    }
}