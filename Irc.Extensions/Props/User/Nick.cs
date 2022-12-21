using Irc.Interfaces;
using Irc.IO;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.User;

internal class Nick : PropRule
{
    private readonly IDataStore _dataStore;

    // limited to 200 bytes including 1 or 2 characters for channel prefix
    public Nick(IDataStore dataStore) : base(ExtendedResources.UserPropNickname, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, string.Empty, true)
    {
        this._dataStore = dataStore;
    }

    public override string GetValue()
    {
        return _dataStore.Get("Name");
    }
}