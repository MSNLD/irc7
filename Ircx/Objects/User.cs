using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;
using Core.Authentication;

namespace Core.Ircx.Objects
{
    public class User : Client
    {
        public UserChannelCollection Channels;
        public UserChannelInfo ActiveChannel;

        public Profile Profile;
        public UserModeCollection Modes;
        public UserAccessLevel Level;
        public Access Access;
        public UserProperties Properties { get { return (UserProperties)base.Properties; } set { base.Properties = value; } }

        public List<UserChannelInfo> ChannelList { get { return Channels.ChannelList;  } }
        public bool IsOnChannel(Channel Channel) {
            //return (this.Channel == Channel);
            for (int c = 0; c < Channels.ChannelList.Count; c++)
            {
                if (Channels.ChannelList[c].Channel == Channel) { return true; }
            }
            return false;
        }

        public User(): base(ObjType.UserObject)
        {
            Access = new Access(Name, false);
            Modes = new UserModeCollection();
            Channels = new UserChannelCollection();
            Properties = new UserProperties(this);

            Profile = new Profile();
        }



        public void BroadcastToChannels(String8 data, bool ExcludeUser)
        {
            for (int c = 0; c < ChannelList.Count; c++)
            {
                Channel channel = ChannelList[c].Channel;
                channel.Send(data, this, ExcludeUser);
            }
        }



        //public void Enqueue(Frame Frame)
        //{
        //    base.FloodProfile.currentInputBytes += (uint)Frame.Message.rawData.length;
        //    base.BufferIn.Queue.Enqueue(Frame);
        //}

        public void AddChannel(UserChannelInfo c)
        {
            ActiveChannel = c;
            Channels.AddChannelInfo(c);
        }
        public void RemoveChannel(Channel c)
        {
            Channels.RemChannel(c);
            if (ActiveChannel != null) { 
                if (ActiveChannel.Channel == c) { ActiveChannel = null; }
            }
        }
        public bool IsMember(Channel Channel)
        {
            if (ActiveChannel.Channel == Channel) { return true; }
            else { return (GetChannelInfo(Channel) != null); }
        }
        public UserChannelInfo GetChannelInfo(Channel Channel)
        {
            if (ActiveChannel != null) { 
                if (ActiveChannel.Channel == Channel) { return ActiveChannel; }
            }

            for (int c = 0; c < Channels.ChannelList.Count; c++)
            {
                if (Channels.ChannelList[c].Channel == Channel) { return Channels.ChannelList[c]; }
            }
            return null;
        }
        public UserChannelInfo GetChannelInfo(String8 Name)
        {
            if (!String8.compareCaseInsensitive(ActiveChannel.Channel.Name,Name)) { return ActiveChannel; }

            for (int c = 0; c < Channels.ChannelList.Count; c++)
            {
                if (!String8.compareCaseInsensitive(Channels.ChannelList[c].Channel.Name,Name)) { return Channels.ChannelList[c]; }
            }
            return null;
        }
    }

    public class UserProperties: PropCollection
    {
        // User Properties
        public Prop Nick = new Prop(Resources.UserPropNickname, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None, true, false);
        public Prop Ircvers = new Prop(Resources.UserPropIrcvers, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None, true, false);
        public Prop Client = new Prop(Resources.UserPropClient, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None, true, false);
        public Prop MsnProfile = new Prop(Resources.UserPropMsnProfile, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.None, true, false);
        public Prop MsnRegCookie = new Prop(Resources.UserPropMsnRegCookie, Resources.Null, 256, UserAccessLevel.NoAccess, UserAccessLevel.None, true, false);
        public Prop Role = new Prop(Resources.UserPropRole, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess, true, false);
        public Prop Puid = new Prop(Resources.UserPropPuid, Resources.Null, 0, UserAccessLevel.None, UserAccessLevel.NoAccess, true, false);

        public UserProperties(Client obj): base(obj)
        {
            base.Properties.Add(Nick);
            base.Properties.Add(Ircvers);
            base.Properties.Add(Client);
            base.Properties.Add(MsnProfile);
            base.Properties.Add(MsnRegCookie);
            base.Properties.Add(Role);
            base.Properties.Add(Puid);
        }
    }

    public class UserChannelInfo
    {
        public ChannelMember Member;
        public Channel Channel;

        public UserChannelInfo(Channel Channel, ChannelMember Member)
        {
            this.Channel = Channel;
            this.Member = Member;
        }
    }

    public class UserChannelCollection
    {
        List<UserChannelInfo> Channels = new List<UserChannelInfo>();
        public List<UserChannelInfo> ChannelList { get { return Channels; } }

        public void AddChannelInfo(UserChannelInfo channel)
        {
            Channels.Add(channel);
        }
        public void RemChannel(Channel channel)
        {
            for (int c = 0; c < Channels.Count; c++)
            {
                if (Channels[c].Channel == channel)
                {
                    Channels.RemoveAt(c);
                    return;
                }
            }
        }
    }

    public class UserCollection: ObjCollection {
        public UserCollection(): base(ObjType.UserObject) { }
        public User this[int c]
        {
            get { return ((User)base.IndexOf(c)); }
        }
        public User GetUser(String8 TargetUser)
        {
            ObjIdentifier objIdentifier = Client.IdentifyObject(TargetUser);
            return ((User)base.FindObj(TargetUser, objIdentifier));
        }
    }
}
