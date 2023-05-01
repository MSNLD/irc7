using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.Constants;

namespace Irc.Extensions.Props.Channel
{
    internal class Subject : PropRule
    {
        public Subject() : base(ExtendedResources.ChannelPropSubject, EnumChannelAccessLevel.ChatMember, EnumChannelAccessLevel.None, Resources.GenericProps, string.Empty, true)
        {

        }
    }
}
