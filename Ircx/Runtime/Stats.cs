using System;
using System.Collections.Generic;
using System.Text;
using Core.Ircx.Objects;
using System.IO;
using System.Xml.Serialization;
using CSharpTools;

namespace Core.Ircx.Runtime
{
    public class category
    {
        public List<channel> channels = new List<channel>();
        [XmlAttribute]
        public string name { get; set; }
    }
    public class channel
    {
        [XmlAttribute]
        public string name;
        [XmlAttribute]
        public string topic;
        [XmlAttribute]
        public string modes;
        [XmlAttribute]
        public string managed;
        [XmlAttribute]
        public string locale;
        [XmlAttribute]
        public string language;
        [XmlAttribute]
        public string currentusers;
        [XmlAttribute]
        public string maxusers;

    }
    public class sitemembers
    {
        public List<sitemember> members = new List<sitemember>();

        [XmlAttribute]
        public string name { get; set; }
    }
    public class sitemember
    {
        [XmlAttribute]
        public string nick;
        public string address;

        public List<string> channels = new List<string>();
    }

    class Stats
    {

        public static void ExportCategories(Server Server)
        {
            string[] sCat = "TN|CP|EV|GN|HE|GE|EA|II|LF|MU|PR|NW|RL|RM|SP|UL".Split(new char[] { '|' }, StringSplitOptions.None);
            for (int c = 0; c < sCat.Length; c++)
            {
                ExportChannelList(sCat[c], Server);
            }

        }
        public static void ExportChannelList(string Category, Server Server)
        {
            category cat = new category();
            cat.name = Category;
            XmlSerializer serializer = new XmlSerializer(typeof(category));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            for (int i = 0; i < Server.Channels.Length; i++)
            {
                //"1:-ST!EN-US!GN"
                string ChannelCategory = (Server.Channels[i].Properties.Subject.Value.ToString()).Split(new char[] { '!' }, StringSplitOptions.None)[2];

                if (ChannelCategory == Category) { 
                    channel c = new channel();
                    c.name = Server.Channels[i].Name.ToString();
                    c.topic = System.Net.WebUtility.UrlEncode(Server.Channels[i].Properties.Topic.Value.ToString());
                    c.modes = (new string(Server.Channels[i].Modes.ChannelModeShortString.ToString().Substring(Server.Channels[i].Modes.ChannelModeShortString.Length))).ToString();
                    c.managed = (Server.Channels[i].Modes.Registered.Value.ToString());
                    c.locale = (Server.Channels[i].Properties.Subject.Value.ToString()).Split(new char[] { '!' }, StringSplitOptions.None)[1];
                    c.language = (Server.Channels[i].Properties.Subject.Value.ToString()).Split(new char[] { ':' }, StringSplitOptions.None)[0];
                    c.currentusers = Server.Channels[i].Members.MemberList.Count.ToString();
                    c.maxusers = Server.Channels[i].Modes.UserLimit.Value.ToString();
                    cat.channels.Add(c);
                }
            }

            if (!Directory.Exists("en-us/ChatFind/Set1/" + Category)) { Directory.CreateDirectory("en-us/ChatFind/Set1/" + Category); }
            using (TextWriter tw = new StreamWriter(File.Create("en-us/ChatFind/Set1/" + Category + "/roomlist.xml"))) {

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
}
