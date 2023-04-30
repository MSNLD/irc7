using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class Name : PropRule
    {
        // limited to 200 bytes including 1 or 2 characters for channel prefix
        public Name() : base(ExtendedResources.ChannelPropName, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.None, string.Empty, true)
        {

        }
    }
}
