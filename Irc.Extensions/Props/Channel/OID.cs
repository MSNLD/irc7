using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class OID : PropRule
    {
        public OID() : base(ExtendedResources.ChannelPropOID, EnumChannelAccessLevel.ChatGuest, EnumChannelAccessLevel.ChatGuest, "0", true)
        {

        }
    }
}
