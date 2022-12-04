namespace Irc.Extensions.Props.Channel;

internal class Subject : PropRule
{
    public Subject() : base(ExtendedResources.ChannelPropSubject, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.None, string.Empty, true)
    {
    }
}