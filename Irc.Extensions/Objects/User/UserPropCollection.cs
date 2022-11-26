using Irc.Extensions.Props.User;
using Irc.IO;
using Irc.Objects.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Objects.User
{
    internal class UserPropCollection: PropCollection
    {
        private readonly IDataStore dataStore;

        public UserPropCollection(IDataStore dataStore): base()
        {
            AddProp(new OID(dataStore));
            AddProp(new Nick(dataStore));
            this.dataStore = dataStore;
        }
    }
}
