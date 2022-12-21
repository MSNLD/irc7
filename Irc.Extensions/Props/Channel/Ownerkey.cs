using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.Channel;

internal class Ownerkey : PropRule
{
    // The OWNERKEY channel property is the owner keyword that will provide owner access when entering the channel. The OWNERKEY property is limited to 31 characters. 
    // It may never be read

    public Ownerkey() : base(ExtendedResources.ChannelPropOwnerkey, EnumChannelAccessLevel.None,
        EnumChannelAccessLevel.ChatOwner, string.Empty)
    {
    }
}