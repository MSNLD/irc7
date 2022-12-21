using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Name : PropRule
{
    // limited to 200 bytes including 1 or 2 characters for channel prefix
    public Name() : base(ExtendedResources.ChannelPropName, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, string.Empty, true)
    {
    }
}