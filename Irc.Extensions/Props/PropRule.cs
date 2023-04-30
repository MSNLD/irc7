using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props
{
    public class PropRule : IPropRule
    {
        public PropRule(string name, EnumChannelAccessLevel readAccessLevel, EnumChannelAccessLevel writeAccessLevel, string initialValue, bool readOnly = false)
        {
            Name = name;
            ReadAccessLevel = readAccessLevel;
            WriteLevel = writeAccessLevel;
            _value = initialValue;
            ReadOnly = readOnly;
        }

        public string Name { get; }
        public EnumChannelAccessLevel ReadAccessLevel { get; }
        public EnumChannelAccessLevel WriteLevel { get; }
        private string _value { get; set; }
        public bool ReadOnly { get; }

        public virtual EnumIrcError SetValue(string value, ChatObject source = null)
        {
            _value = value;
            return EnumIrcError.OK;
        }

        public virtual string GetValue() => _value;
    }
}
