using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class Name : PropRule
    {
        // limited to 200 bytes including 1 or 2 characters for channel prefix
        public Name() : base(ExtendedResources.ChannelPropName, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.None, Resources.GenericProps, string.Empty, true)
        {

        }
    }
}
