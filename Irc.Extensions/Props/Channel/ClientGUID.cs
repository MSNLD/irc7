using Irc.Constants;
using Irc.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class ClientGUID : PropRule
    {
        // The CLIENTGUID channel property contains a GUID that defines the client protocol to be used within the channel.
        // This property may be set and read like the LAG property. 
        public ClientGUID() : base(ExtendedResources.ChannelPropClient, EnumChannelAccessLevel.None, EnumChannelAccessLevel.None, Resources.GenericProps, string.Empty, true)
        {

        }
    }
}
