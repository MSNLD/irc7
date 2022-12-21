using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Hostkey : PropRule
{
    // The HOSTKEY channel property is the host keyword that will provide host (channel op) access when entering the channel. 
    // It may never be read.
    public Hostkey() : base(ExtendedResources.ChannelPropHostkey, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatHost, string.Empty)
    {
    }
}