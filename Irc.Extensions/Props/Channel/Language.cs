﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class Language : PropRule
    {
        // The LANGUAGE channel property is the preferred language type. The LANGUAGE property is a string limited to 31 characters. 
        public Language() : base(ExtendedResources.ChannelPropLanguage, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.ChatHost, Resources.GenericProps, string.Empty, false)
        {

        }
    }
}
