﻿using Irc.Constants;
using Irc.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Interfaces;

namespace Irc.Extensions.Props.User
{
    internal class OID : PropRule
    {
        private readonly IDataStore dataStore;

        public OID(IDataStore dataStore) : base(ExtendedResources.UserPropOid, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.None, Resources.GenericProps, "0", true)
        {
            this.dataStore = dataStore;
        }

        public override string GetValue(IChatObject target)
        {
            return dataStore.Get(Resources.UserPropOid);
        }
    }
}
