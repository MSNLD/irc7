using Irc.Enumerations;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Modes
{
    public class ModeRule
    {
        public char ModeChar { get; }
        public int ModeValue { get; set; }

        public ModeRule(char modeChar, int modeValue = 0)
        {
            ModeChar = modeChar;
            ModeValue = modeValue;
        }

        // Although the below is a string we are to evaluate and cast to integer
        // We can also throw bad value here if it is not the desired type
        public EnumModeResult Evaluate(ChatObject chatObject, string modeValue)
        {
            throw new NotSupportedException();
        }
    }
}
