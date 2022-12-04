using Irc.Extensions.Interfaces;
using Irc.IO;

namespace Irc.Extensions.Props.User;

internal class Nick : PropRule, IPropRule
{
    private readonly IDataStore dataStore;

    // limited to 200 bytes including 1 or 2 characters for channel prefix
    public Nick(IDataStore dataStore) : base(ExtendedResources.UserPropNickname, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, string.Empty, true)
    {
        this.dataStore = dataStore;
    }

    public override string GetValue()
    {
        return dataStore.Get("Name");
    }
}