namespace Irc.Extensions.Props.Channel;

internal class Client : PropRule
{
    // The CLIENT channel property contains client-specified information.
    // The format is not defined by the server.
    // The CLIENT property is limited to 255 characters.
    // This property may be set and read like the TOPIC property.
    public Client() : base(ExtendedResources.ChannelPropClient, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatHost, string.Empty, true)
    {
    }
}