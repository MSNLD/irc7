﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class OID : PropRule
    {
        public OID() : base(ExtendedResources.ChannelPropOID, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.None, Resources.ChannelPropOIDRegex, "0", true)
        {

        }
    }
}
