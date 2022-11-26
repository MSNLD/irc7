using Irc.Enumerations;
using Irc.Extensions.Interfaces;
using Irc.IO;
using Irc.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Objects
{
    public class xxExtendedChatObject : ChatObject, IExtendedChatObject
    {
        public xxExtendedChatObject(IModeCollection modes, IDataStore dataStore, IPropCollection propCollection) : base(modes, dataStore)
        {
            PropCollection = propCollection;
        }

        public IPropCollection PropCollection { get; }
        public IAccessList AccessList { get; }
    }
}
