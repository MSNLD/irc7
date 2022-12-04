namespace Irc.Extensions.Props.Channel;

internal class OID : PropRule
{
    public OID() : base(ExtendedResources.ChannelPropOID, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, "0", true)
    {
    }
}