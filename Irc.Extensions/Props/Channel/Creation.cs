namespace Irc.Extensions.Props.Channel;

internal class Creation : PropRule
{
    // The CREATION channel property is the time that the channel was created, in number of seconds elapsed since midnight (00:00:00), January 1, 1970, (coordinated universal time)
    public Creation() : base(ExtendedResources.ChannelPropCreation, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, string.Empty, true)
    {
    }
}