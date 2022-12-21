using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class ServicePath : PropRule
{
    public ServicePath() : base(ExtendedResources.ChannelPropServicePath, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatOwner, string.Empty, true)
    {
    }
}