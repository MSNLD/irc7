using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class ServicePath : PropRule
    {
        public ServicePath() : base(ExtendedResources.ChannelPropServicePath, EnumChannelAccessLevel.None, EnumChannelAccessLevel.ChatOwner, string.Empty, true)
        {

        }
    }
}
