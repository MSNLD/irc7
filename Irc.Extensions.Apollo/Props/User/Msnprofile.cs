using Irc.Extensions.Apollo.Objects.User;
using Irc.Extensions.Props;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Apollo.Props.User;

internal class Msnprofile : PropRule
{
    private readonly ApolloProfile _profile;

    public Msnprofile(ApolloProfile profile) : base(ExtendedResources.UserPropMsnProfile,
        EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatMember, "0", true)
    {
        this._profile = profile;
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
        _profile.SetProfileCode(profileCode);
    }
}