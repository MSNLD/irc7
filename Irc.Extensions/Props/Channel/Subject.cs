using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Props.Channel
{
    internal class Subject : PropRule
    {
        public Subject() : base(ExtendedResources.ChannelPropSubject, EnumChannelAccessLevel.ChatGuest, EnumChannelAccessLevel.None, string.Empty, true)
        {

        }
    }
}
