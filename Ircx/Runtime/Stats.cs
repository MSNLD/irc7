using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Core.Ircx.Objects;

namespace Core.Ircx.Runtime;

public class category
{
    public List<channel> channels = new();

    [XmlAttribute] public string name { get; set; }
}

public class channel
{
    [XmlAttribute] public string currentusers;

    [XmlAttribute] public string language;

    [XmlAttribute] public string locale;

    [XmlAttribute] public string managed;

    [XmlAttribute] public string maxusers;

    [XmlAttribute] public string modes;

    [XmlAttribute] public string name;

    [XmlAttribute] public string topic;
}

public class sitemembers
{
    public List<sitemember> members = new();

    [XmlAttribute] public string name { get; set; }
}

public class sitemember
{
    public string address;

    public List<string> channels = new();

    [XmlAttribute] public string nick;
}

internal class Stats
{
    public static void ExportCategories(Server Server)
    {
        var sCat = "TN|CP|EV|GN|HE|GE|EA|II|LF|MU|PR|NW|RL|RM|SP|UL".Split(new[] {'|'}, StringSplitOptions.None);
        for (var c = 0; c < sCat.Length; c++) ExportChannelList(sCat[c], Server);
    }

    public static void ExportChannelList(string Category, Server Server)
    {
        var cat = new category();
        cat.name = Category;
        var serializer = new XmlSerializer(typeof(category));
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        for (var i = 0; i < Server.Channels.Length; i++)
        {
            //"1:-ST!EN-US!GN"
            var ChannelCategory =
                Server.Channels[i].Properties.Subject.Value.Split(new[] {'!'}, StringSplitOptions.None)[2];

            if (ChannelCategory == Category)
            {
                var c = new channel();
                c.name = Server.Channels[i].Name;
                c.topic = WebUtility.UrlEncode(Server.Channels[i].Properties.Topic.Value);
                c.modes = new string(Server.Channels[i].Modes.ChannelModeShortString
                    .Substring(Server.Channels[i].Modes.ChannelModeShortString.Length));
                c.managed = Server.Channels[i].Modes.Registered.Value.ToString();
                c.locale = Server.Channels[i].Properties.Subject.Value.Split(new[] {'!'}, StringSplitOptions.None)[1];
                c.language = Server.Channels[i].Properties.Subject.Value.Split(new[] {':'}, StringSplitOptions.None)[0];
                c.currentusers = Server.Channels[i].Members.MemberList.Count.ToString();
                c.maxusers = Server.Channels[i].Modes.UserLimit.Value.ToString();
                cat.channels.Add(c);
            }
        }

        if (!Directory.Exists("en-us/ChatFind/Set1/" + Category))
            Directory.CreateDirectory("en-us/ChatFind/Set1/" + Category);
        using (TextWriter tw = new StreamWriter(File.Create("en-us/ChatFind/Set1/" + Category + "/roomlist.xml")))
        {
            serializer.Serialize(tw, cat, ns);
        }
    }

    public static void ExportUserList(Server Server)
    {
        // TODO: Fix this
        //sitemembers members = new sitemembers();
        //members.name = "Members";
        //XmlSerializer serializer = new XmlSerializer(typeof(sitemembers));
        //XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        //ns.Add("", "");
        //for (int i = 0; i < Server.Users.Length; i++)
        //{
        //    sitemember sm = new sitemember();
        //    sm.nick = Server.Users[i].Name.chars;
        //    if (Server.Users[i].Address != null) { 
        //        if (Server.Users[i].Address._address != null) { 
        //            if (Server.Users[i].Address._address.Length == 5)
        //            {
        //                sm.address = Server.Users[i].Address._address[1].chars;
        //            }
        //        }
        //    }

        //    for (int x = 0; x < Server.Users[i].ChannelList.Count; x++)
        //    {
        //        sm.channels.Add(Server.Users[i].ChannelList[x].Channel.Name.chars);
        //    }

        //    members.members.Add(sm);
        //}

        //if (!Directory.Exists("en-us/Users/")) { Directory.CreateDirectory("en-us/Users/"); }
        //using (TextWriter tw = new StreamWriter(File.Create("en-us/Users/" + "/roomlist.xml")))
        //{
        //    serializer.Serialize(tw, members, ns);
        //}
    }
}