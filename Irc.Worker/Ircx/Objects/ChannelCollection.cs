using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public class ChannelCollection : ObjCollection
{
    public ChannelCollection() : base(ObjType.ChannelObject)
    {
    }

    public Channel this[int c] => (Channel) IndexOf(c);

    public Channel GetChannel(string TargetChannel)
    {
        var objIdentifier = Obj.IdentifyObject(TargetChannel);
        return (Channel) FindObj(TargetChannel, objIdentifier);
    }

    public void RemoveEmptyChannels()
    {
        for (var i = ObjectCollection.Count - 1; i >= 0; i--)
        {
            var c = (Channel) ObjectCollection[i];
            if (c.MemberList.Count == 0 && c.Modes.Registered.Value != 0x1) ObjectCollection.RemoveAt(i);
        }
    }

    public List<Channel> GetChannels(Server Server, User User, string ChannelNames, bool ReportMissing)
    {
        var ChannelList = Tools.CSVToArray(ChannelNames);
        if (ChannelList == null) return null;

        // Clear out garbage first
        for (var x = 0; x < ChannelList.Count; x++)
            if (!Channel.IsChannel(ChannelList[x]))
            {
                if (ReportMissing)
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {ChannelList[x]}));
                ChannelList.RemoveAt(x);
                x--;
            }

        var Channels = new List<Channel>();

        for (var c = 0; c < Server.Channels.Length; c++)
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
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {ChannelList[x]}));

        return Channels;
    }
}