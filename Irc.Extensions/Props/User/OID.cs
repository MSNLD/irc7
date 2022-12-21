using Irc.Interfaces;
using Irc.IO;
using Irc.Models.Enumerations;

namespace Irc.Extensions.Props.User;

internal class Oid : PropRule
{
    private readonly IDataStore _dataStore;

    public Oid(IDataStore dataStore) : base(ExtendedResources.UserPropOid, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, "0", true)
    {
        this._dataStore = dataStore;
    }

    public override string GetValue()
    {
        return _dataStore.Get("ObjectId");
    }
}