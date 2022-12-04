using Irc.IO;

namespace Irc.Extensions.Props.User;

internal class OID : PropRule
{
    private readonly IDataStore dataStore;

    public OID(IDataStore dataStore) : base(ExtendedResources.UserPropOid, EnumChannelAccessLevel.ChatMember,
        EnumChannelAccessLevel.ChatMember, "0", true)
    {
        this.dataStore = dataStore;
    }

    public override string GetValue()
    {
        return dataStore.Get("ObjectId");
    }
}