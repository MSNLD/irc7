using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irc.ClassExtensions.CSharpTools;
using Irc.Constants;
using Irc.Objects;
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

            for (var c = 0; c < Server.Channels.Count; c++)
            for (var x = 0; x < ChannelList.Count; x++)
                if (Server.Channels[c].Name.ToUpper() == ChannelList[x].ToUpper())
                {
                    Channels.Add(Server.Channels[c]);
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

        public static long GetCreationDate() => (DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond;

        public static T FindObj<T>(this IList<T> objectList, string Name, IrcHelper.ObjIdentifier objectType) where T : ChatObject
        {
            switch (objectType)
            {
                case IrcHelper.ObjIdentifier.ObjIdInternal:
                {
                    // Search as OID
                    return objectList.FindObjByOID(Name);
                }
                case IrcHelper.ObjIdentifier.ObjIdIRCUserHex:
                {
                    // Search as HEX
                    return objectList.FindObjByHex(Name);
                }
                default:
                {
                    // Search by Name
                    return objectList.FindObjByName(Name);
                }
            }
        }

        public static T FindObjByOID<T>(this IList<T> objectList, string objectId) where T : ChatObject
        {
            foreach (var obj in objectList)
                if (obj.Id.ToString() == objectId)
                    return obj;

            return default(T);
        }

        public static T FindObjByHex<T>(this IList<T> objectList, string Hex) where T : ChatObject
        {
            var HexString = new string(Hex.Substring(1));

            HexString = Tools.HexToString(HexString);

            return objectList.FindObjByName(HexString);
        }

        public static T FindObjByName<T>(this IList<T> objectList, string Name) where T : ChatObject
        {
            for (var c = 0; c < objectList.Count; c++)
                if (objectList[c].Name.ToUpper() == Name.ToUpper())
                    return objectList[c];
            return default(T);
        }
    }
}
