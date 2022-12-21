using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Oid : PropRule
{
    public Oid() : base(ExtendedResources.ChannelPropOid, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, "0", true)
    {
    }
}