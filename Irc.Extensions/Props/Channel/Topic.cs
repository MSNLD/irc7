namespace Irc.Extensions.Props.Channel;

internal class Topic : PropRule
{
    public Topic() : base(ExtendedResources.ChannelPropTopic, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatHost, string.Empty)
    {
    }
}