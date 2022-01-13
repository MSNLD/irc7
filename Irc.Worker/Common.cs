using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Worker.Ircx;
using Irc.Worker.Ircx.Objects;

namespace Irc.Worker
{
    internal static class Common
    {
        public static List<Channel> GetChannels(Server Server, User User, string ChannelNames, bool ReportMissing)
        {
            var ChannelList = Tools.CSVToArray(ChannelNames);
            if (ChannelList == null) return null;

            // Clear out garbage first
            for (var x = 0; x < ChannelList.Count; x++)
                if (!Channel.IsChannel(ChannelList[x]))
                {
                    if (ReportMissing)
                        User.Send(RawBuilder.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                            Data: new[] { ChannelList[x] }));
                    ChannelList.RemoveAt(x);
                    x--;
                }

            var Channels = new List<Channel>();

            for (var c = 0; c < Server.Channels.Length; c++)
            for (var x = 0; x < ChannelList.Count; x++)
                if (Server.Channels.IndexOf(c).Name.ToUpper() == ChannelList[x].ToUpper())
                {
                    Channels.Add(Server.Channels.IndexOf(c));
                    // Once found narrow the search further to save cycles
                    ChannelList.RemoveAt(x);
                    x--;
                }

            // Report no such channels
            if (ChannelList.Count > 0)
                for (var x = 0; x < ChannelList.Count; x++)
                    if (ReportMissing)
                        User.Send(RawBuilder.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                            Data: new[] { ChannelList[x] }));

            return Channels;
        }
    }
}
