using Irc.Extensions.Interfaces;
using Irc.Extensions.Props.Channel;
using Irc.IO;
using Irc.Objects.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Objects.Channel
{
    internal class ChannelPropCollection: PropCollection
    {
        public ChannelPropCollection(): base()
        {
            AddProp(new OID());
            AddProp(new Name());
            AddProp(new Creation());
            AddProp(new Language());
            AddProp(new Ownerkey());
            AddProp(new Hostkey());
            AddProp(new Memberkey());
            AddProp(new Pics());
            AddProp(new Topic());
            AddProp(new Subject());
            AddProp(new Onjoin());
            AddProp(new Onpart());
            AddProp(new Lag());
            AddProp(new Client());
            AddProp(new ClientGUID());
            AddProp(new ServicePath());
        }
    }
}
