using Irc.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.User
{
    internal class OID : PropRule
    {
        private readonly IDataStore dataStore;

        public OID(IDataStore dataStore) : base(ExtendedResources.UserPropOid, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatMember, "0", true)
        {
            this.dataStore = dataStore;
        }

        public override string GetValue()
        {
            return dataStore.Get("ObjectId");
        }
    }
}
