using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class ClientGuid : PropRule
{
    // The CLIENTGUID channel property contains a GUID that defines the client protocol to be used within the channel.
    // This property may be set and read like the LAG property. 
    public ClientGuid() : base(ExtendedResources.ChannelPropClient, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.None, string.Empty, true)
    {
    }
}