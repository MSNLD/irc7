namespace Irc.Extensions.Props.Channel;

internal class Memberkey : PropRule
{
    // The MEMBERKEY channel property is the keyword required to enter the channel. The MEMBERKEY property is limited to 31 characters. 
    // It may never be read.
    public Memberkey() : base(ExtendedResources.ChannelPropMemberkey, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatHost, string.Empty)
    {
    }
}