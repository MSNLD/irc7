using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class ServicePath : PropRule
    {
        public ServicePath() : base(ExtendedResources.ChannelPropServicePath, EnumChannelAccessLevel.None, EnumChannelAccessLevel.ChatOwner, Resources.GenericProps, string.Empty, true)
        {

        }
    }
}
