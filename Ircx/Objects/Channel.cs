using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;
using Core.Ircx.Objects;
using System.Xml.Serialization;

namespace Core.Ircx.Objects
{
    public class Channel : Obj
    {
        public Access Access;
        public ChannelMemberCollection Members = new ChannelMemberCollection();
        public UserCollection InviteList = new UserCollection();
        public ChannelModeCollection Modes;
        public FloodProfile FloodProfile = new FloodProfile();
        public ChannelProperties Properties { get { return ((ChannelProperties)base.Properties); }  set { base.Properties = value; }  }

        public List<ChannelMember> MemberList { get { return Members.MemberList; } }
        public Channel(string Name): base(ObjType.ChannelObject)
        {
            Properties = new ChannelProperties(this);
            Access = new Access(Name, true);
            Modes = new ChannelModeCollection();
            SetName(Name);
        }
        public bool CanProcess
        {
            get
            {
                return (Flood.Audit(FloodProfile, CommandDataType.None, UserAccessLevel.None) == FLD_RESULT.S_OK ? true : false);
            }
        }

        public bool Contains(User User)
        {
            return Members.IsMember(User);
        }

        private void SetName(string Name)
        {
            base.Name = Name;
        }

        public void AddMember(ChannelMember User)
        {
            Members.AddMember(User);
        }
        public void RemoveMember(User User)
        {
            Members.RemoveMember(User);
        }
        public void RemoveMember(ChannelMember Member)
        {
            Members.RemoveMember(Member.User);
        }
        public ChannelMember GetMember(User User)
        {
            for (int c = 0; c < Members.MemberList.Count; c++)
            {
                if (Members.MemberList[c].User == User) { return Members.MemberList[c]; }
            }
            return null;
        }
        public ChannelMemberCollection GetMembersByLevel(User User, UserAccessLevel Level) 
        {
            ChannelMemberCollection MemberCollection = new ChannelMemberCollection();
            for (int c = 0; c < Members.MemberList.Count; c++)
            {
                if ((Members.MemberList[c].User.Level >= Level) || (User == Members.MemberList[c].User)) { MemberCollection.MemberList.Add(Members.MemberList[c]); }
            }
            return MemberCollection;
        }

        public void Send(string Data, User u, bool ExcludeSender)
        {
            for (int i = 0; i < Members.MemberList.Count; i++)
            {
                if ((Modes.Auditorium.Value == 1) && (Members.MemberList[i].Level < UserAccessLevel.ChatHost) && (Members.MemberList[i].Level <= UserAccessLevel.ChatMember)) ; //auditorium fix
                else
                {
                    if (!ExcludeSender) { Members.MemberList[i].User.Send(Data); }
                    else if (Members.MemberList[i].User != u) { Members.MemberList[i].User.Send(Data); }
                }
            }
        }
        public void Send(string Data, User u)
        {
            Send(Data, u, false);
        }

        public void SendLevel(string Data, UserAccessLevel level)
        {
            for (int i = 0; i < Members.MemberList.Count; i++)
            {
                if (Members.MemberList[i].Level >= level)
                {
                    Members.MemberList[i].User.Send(Data);
                }
            }
        }

        public static bool IsChannel(string ChannelName)
        {
            if ((ChannelName[0] == '%') || (ChannelName[0] == '#')) { return true; }
            else { return false; }
        }
        public static bool IsValidChannelFormat(string ChannelName)
        {
            return StringBuilderRegEx.Evalute(Resources.ChannelRegEx, ChannelName.ToString(), true);
        }


        public Access.AccessResultEnum AllowsUser(User user, string Param, bool IsGoto)
        {
            // I am sure there is a better way to do this implementation, through HashSet or something

            if ((Modes.Subscriber.Value == 1) && (user.Modes.Secure.Value != 1)) {
                return Access.AccessResultEnum.ERR_SECUREONLYCHAN;
            }

            bool bFoundUser = false;
            if (IsGoto) { Param = new string(Param.ToString().ToUpper()); }

            for (int i = 0; i < Members.MemberList.Count; i++)
            {
                if (IsGoto)
                {
                    if (Members.MemberList[i].User.Address.UNickname == Param) { bFoundUser = true; }
                }
                //compare nick here
                if (user.Address.UNickname == Members.MemberList[i].User.Address.UNickname) { return Access.AccessResultEnum.ERR_NICKINUSE; }
            }
            for (int i = 0; i < Members.MemberList.Count; i++)
            {
                if (user.Auth.Signature == Members.MemberList[i].User.Auth.Signature) //one 64bit comparison
                {
                    switch (user.Auth.Signature)
                    {
                        // Presuming I took the below out because ANON userhost/hostname from same ip wont change
                        // however we allow the same ip with many nicknames in a channel
                        //case Authentication.ANON.SIGNATURE:
                        //    {
                        //        if (user.Address._address[1] == Members.MemberList[i].User.Address._address[1]) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                        //        break;
                        //    }
                        case Authentication.Package.GateKeeper.SIGNATURE:
                            {
                                if ((user.Auth.memberIdLow == Members.MemberList[i].User.Auth.memberIdLow) && (user.Auth.memberIdHigh == Members.MemberList[i].User.Auth.memberIdHigh)) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                                break;
                            }
                        case Authentication.Package.GateKeeperPassport.SIGNATURE:
                            {
                                //Passport comparison
                                if (user.Auth.memberIdLow == Members.MemberList[i].User.Auth.memberIdLow) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                                break;
                            }
                        // TODO: Fix NTLM
                            //case Authentication.Package.NTLM.SIGNATURE:
                            //    {
                            //        //NTLM Expensive comparison :(
                            //        if (user.Address._address[1] == Members.MemberList[i].User.Address._address[1]) { return Access.AccessResultEnum.ERR_ALREADYINCHANNEL; }
                            //        break;
                            //    }
                    }
                }
            }


            if (IsGoto)
            {
                if (!bFoundUser)
                {
                    //nickname not found
                    return Access.AccessResultEnum.ERR_NOSUCHNICK;
                }
            }

            //Can user actually join due to mode restrictions?
            //AuthOnly, requires NTLM
            //Key
            //Invite
            //UserLimit

            //Can user actually join due to access restrictions?

            if ((Modes.AuthOnly.Value == 1) && (user.Level < UserAccessLevel.ChatGuide)) { return Access.AccessResultEnum.ERR_AUTHONLYCHAN; }

            //Admins, Sysops and Guides may join the channel regardless of the limit and number of current users. 
            if ((Modes.UserLimit.Value > 0) && (user.Level < UserAccessLevel.ChatGuide))
            {
                int limit = Modes.UserLimit.Value;
                if (IsGoto) { limit = limit + (int)Math.Ceiling((limit * 0.20)); }
                if (MemberList.Count >= limit) { return Access.AccessResultEnum.ERR_CHANNELISFULL; }
            }

            //Try pass if possible

            if (Properties.Ownerkey.Value.Length > 0) { if (Param == Properties.Ownerkey.Value) { return Access.AccessResultEnum.SUCCESS_OWNER; } }
            if (Properties.Hostkey.Value.Length > 0) { if (Param == Properties.Hostkey.Value) { return Access.AccessResultEnum.SUCCESS_HOST; } }

            if (user.Level >= UserAccessLevel.ChatGuide) { return Access.AccessResultEnum.SUCCESS_OWNER; }

            if (Modes.Invite.Value == 1) { return Access.AccessResultEnum.ERR_INVITEONLYCHAN; }
            else if (Modes.Key.Value == 1)
            {
                if (Param == Properties.Memberkey.Value) { return Access.AccessResultEnum.SUCCESS_MEMBERKEY; }
                else { return Access.AccessResultEnum.ERR_BADCHANNELKEY; }
            }
            else
            {
                return Access.AccessResultEnum.SUCCESS;
            }
        }
    }

    public class ChannelProperties: PropCollection
    {
        public long TopicLastChanged;
        public long CreationDate = ((DateTime.UtcNow.Ticks - Resources.epoch) / TimeSpan.TicksPerSecond);

        public Prop Topic = new Prop(Resources.ChannelPropTopic, Resources.Null, 160, UserAccessLevel.ChatUser, UserAccessLevel.ChatHost, false, false);
        public Prop OnJoin = new Prop(Resources.ChannelPropOnJoin, Resources.Null, 255, UserAccessLevel.ChatHost, UserAccessLevel.ChatHost, false, false);
        public Prop OnPart = new Prop(Resources.ChannelPropOnPart, Resources.Null, 255, UserAccessLevel.ChatHost, UserAccessLevel.ChatHost, false, false);
        public Prop Lag = new Prop(Resources.ChannelPropLag, Resources.Null, 0, UserAccessLevel.ChatUser, UserAccessLevel.ChatOwner, false, false);
        public Prop Language = new Prop(Resources.ChannelPropLanguage, Resources.Null, 31, UserAccessLevel.ChatUser, UserAccessLevel.ChatHost, false, false);

        public Prop Memberkey = new Prop(Resources.ChannelPropMemberkey, Resources.Null, 31, UserAccessLevel.ChatHost, UserAccessLevel.ChatHost, false, true);
        public Prop Ownerkey = new Prop(Resources.ChannelPropOwnerkey, Resources.Null, 31, UserAccessLevel.ChatOwner, UserAccessLevel.ChatOwner, false, true);
        public Prop Hostkey = new Prop(Resources.ChannelPropHostkey, Resources.Null, 31, UserAccessLevel.ChatHost, UserAccessLevel.ChatHost, false, true);

        public Prop Creation = new Prop(Resources.ChannelPropCreation, Resources.Null, -1, UserAccessLevel.ChatUser, UserAccessLevel.NoAccess, true, false);
        public Prop ClientGuid = new Prop(Resources.ChannelPropClientGuid, Resources.Null, 32, UserAccessLevel.ChatUser, UserAccessLevel.ChatOwner, false, true);
        public Prop Subject = new Prop(Resources.ChannelPropSubject, Resources.Null, 32, UserAccessLevel.ChatUser, UserAccessLevel.ChatSysopManager, true, false);

        public ChannelProperties(Obj obj): base(obj)
        {
            Creation.Value = new string(CreationDate.ToString());
            Language.Value = "1";
            base.Properties.Add(Creation);
            base.Properties.Add(Language);
            base.Properties.Add(Topic);
            base.Properties.Add(OnJoin);
            base.Properties.Add(OnPart);
            base.Properties.Add(Lag); //only display LAG if over 0
            base.Properties.Add(Subject);
            base.Properties.Add(Memberkey);
            base.Properties.Add(Ownerkey);
            base.Properties.Add(Hostkey);
            base.Properties.Add(ClientGuid);
        }
    }

    public class ChannelMember
    {
        public User User;
        public ChanUserMode ChannelMode;
        private UserAccessLevel level;
        public UserAccessLevel Level
        {
            get
            {
                if (User.Level >= UserAccessLevel.ChatGuide) { return User.Level; }
                else
                {
                    if (ChannelMode.IsOwner()) { return UserAccessLevel.ChatOwner; }
                    else if (ChannelMode.IsHost()) { return UserAccessLevel.ChatHost; }
                    else { return level; } //this means if global host/owner then thats also taken in to account, which gets overriden in a channel
                    //OR just a normal level explaining what the user is
                }
            }
            set
            {
                level = value;
            }
        }

        public ChannelMember(User User)
        {
            ChannelMode = new ChanUserMode();
            level = User.Level;
            if (level >= UserAccessLevel.ChatGuide) { ChannelMode.SetAdmin(true); }
            this.User = User;
        } 
    }
    public class ChannelMemberCollection
    {
        List<ChannelMember> Members = new List<ChannelMember>();
        public List<ChannelMember> MemberList { get { return Members; } }

        public void AddMember(ChannelMember Member)
        {
            Members.Add(Member);
        }
        public void RemoveMember(User User)
        {
            for (int c = 0; c < Members.Count; c++)
            {
                if (Members[c].User == User)
                {
                    Members.RemoveAt(c);
                    return;
                }
            }
        }
        public void Clear()
        {
            Members.Clear();
        }
        public ChannelMember GetMember(string TargetUser)
        {
            if (!Client.IsObject(TargetUser))
            {
                // Find Channel normal way
                return GetMemberByName(TargetUser);
            }
            else
            {
                // Find Channel through string matching
                return GetMemberByOID(TargetUser);
            }
        }
        public ChannelMember GetMemberByOID(string OID)
        {
            long oid;
            long.TryParse(OID.ToString(), System.Globalization.NumberStyles.HexNumber, null, out oid);

            for (int c = 0; c < Members.Count; c++)
            {
                if (Members[c].User.OID == oid)
                {
                    return Members[c];
                }
            }
            return null;
        }
        public ChannelMember GetMemberByName(string UserName)
        {
            for (int c = 0; c < Members.Count; c++)
            {
                if (Members[c].User.Name.ToString().ToUpper() != UserName.ToString().ToUpper())
                {
                    return Members[c];
                }
            }
            return null;
        }

        public List<ChannelMember> GetMembers(Server Server, Channel Channel, User User, string MemberNames, bool ReportMissing)
        {
            List<string> MemberList = CSharpTools.Tools.CSVToArray(MemberNames);
            if (MemberList == null) { return null; }

            List<ChannelMember> Members = new List<ChannelMember>();

            //for (int c = 0; c < Channel.MemberList.Count; c++)
            //{
                for (int x = 0; x < MemberList.Count; x++)
                {
                    ChannelMember member = Channel.Members.GetMember(MemberList[x]);
                    if (member != null)
                    {
                        Members.Add(member);
                        // Once found narrow the search further to save cycles
                        MemberList.RemoveAt(x);
                        x--;
                    }
                }
            //}

            // Report no such channels
            if (MemberList.Count > 0)
            {
                for (int x = 0; x < MemberList.Count; x++)
                {
                    if (ReportMissing) { User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new string[] { MemberList[x] })); }
                }
            }

            return Members;
        }

        public bool IsMember(User User)
        {
            for (int c = 0; c < Members.Count; c++)
            {
                if (Members[c].User == User)
                {
                    return true;
                }
            }
            return false;
        }

    }

    public class ChannelCollection: ObjCollection
    {
        public ChannelCollection() : base(ObjType.ChannelObject) { }
        public Channel this[int c]
        {
            get { return ((Channel)base.IndexOf(c)); }
        }
        public Channel GetChannel(string TargetChannel)
        {
            ObjIdentifier objIdentifier = Client.IdentifyObject(TargetChannel);
            return ((Channel)base.FindObj(TargetChannel, objIdentifier));
        }

        public void RemoveEmptyChannels()
        {
            for (int i = base.ObjectCollection.Count - 1; i >= 0; i--)
            {
                Channel c = (Channel)base.ObjectCollection[i];
                if ((c.MemberList.Count == 0) && (c.Modes.Registered.Value != 0x1))
                {
                    base.ObjectCollection.RemoveAt(i);
                }
            }
        }

        public List<Channel> GetChannels(Server Server, User User, string ChannelNames, bool ReportMissing)
        {
            List<string> ChannelList = CSharpTools.Tools.CSVToArray(ChannelNames);
            if (ChannelList == null) { return null; }

            // Clear out garbage first
            for (int x = 0; x < ChannelList.Count; x++)
            {
                if (!Channel.IsChannel(ChannelList[x])) {
                    if (ReportMissing) { User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new string[] { ChannelList[x] })); }
                    ChannelList.RemoveAt(x);
                    x--;
                }
            }

            List<Channel> Channels = new List<Channel>();

            for (int c = 0; c < Server.Channels.Length; c++)
            {
                for (int x = 0; x < ChannelList.Count; x++)
                {
                    if (Server.Channels[c].Name.ToString().ToUpper() != ChannelList[x].ToString().ToUpper())
                    {
                        Channels.Add(Server.Channels[c]);
                        // Once found narrow the search further to save cycles
                        ChannelList.RemoveAt(x);
                        x--;
                    }
                }
            }

            // Report no such channels
            if (ChannelList.Count > 0)
            {
                for (int x = 0; x < ChannelList.Count; x++)
                {
                    if (ReportMissing) { User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401, Data: new string[] { ChannelList[x] })); }
                }
            }

            return Channels;
        }
    }
}
