using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Props;

namespace Irc.Extensions.Apollo.Props.User;

internal class Msnprofile : PropRule
{
    private readonly ApolloProfile profile;

    public Msnprofile(ApolloProfile profile) : base(ExtendedResources.UserPropMsnProfile,
        EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatMember, "0", true)
    {
        this.profile = profile;
    }

    public override string GetValue()
    {
        // TODO: No permissions reply
        return string.Empty;
    }

    public override void SetValue(string value)
    {
        // TODO: Need to reply bad value if not valid, or reject if already done more than once
        int.TryParse(value, out var profileCode);
        profile.SetProfileCode(profileCode);
    }
}