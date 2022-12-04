using Irc.Extensions.Interfaces;
using Irc.IO;
using Irc.Objects;

namespace Irc.Extensions.Objects;

public class xxExtendedChatObject : ChatObject, IExtendedChatObject
{
    public xxExtendedChatObject(IModeCollection modes, IDataStore dataStore, IPropCollection propCollection) : base(
        modes, dataStore)
    {
        PropCollection = propCollection;
    }

    public IPropCollection PropCollection { get; }
    public IAccessList AccessList { get; }
}