using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;
using Core.Ircx;
using Core.Ircx.Commands;
using Core.Ircx.Objects;

/*You must specify the protocol as IRC3 if you want 
to use the additional MODE flag of +h (see below).
 * 
 * Obviously make an exception for IRC0 (mIRC compliant) and IRC9 (MSN compliant) experimental ircvers
 * Actually this means that IRC1-2 cannot use it, which will be funny
*/

namespace Core
{
    public abstract class ModeFunction
    {
    }
    public abstract class ChannelModeFunction : ModeFunction
    {
        public abstract bool Execute(Server server, Channel channel, User user, bool bModeFlag, ref AuditModeReport report, Message message);
    }
    public abstract class UserModeFunction : ModeFunction
    {
        public abstract bool Execute(Server server, User user, User TargetUser, bool bModeFlag, ref AuditModeReport report, Message message);
    }
    public class Mode
    {
        public byte ModeChar;
        public int Value;
        public uint Level;
        public ModeFunction Function;

        public Mode(byte ModeChar, int Value, uint Level, ModeFunction Function)
        {
            this.ModeChar = ModeChar;
            this.Value = Value;
            this.Level = Level;
            this.Function = Function;
        }
        public Mode() { }
    }

    //Upper tiers need
    //public enum ChanUserMode { NORMAL = 0, VOICE = (byte)'+', HOST = (byte)'@', OWNER = (byte)'.' };
    public class ChanUserMode
    {
        public static uint Admin = 0xFF000000;
        public static uint Owner = 0x00FF0000;
        public static uint Host = 0x0000FF00;
        public static uint Voice = 0x000000FF;
        public static uint Normal = 0x0;
        public byte modeChar;


        public uint UserMode; // 0x0 = NORMAL, 0xFF00 = VOICE, 0xFF0000 = HOST, 0xFF000000 = OWNER

        public bool IsAdmin() { return ((Admin & UserMode) == Admin); }
        public bool IsOwner() { return ((Owner & UserMode) == Owner); }
        public bool IsHost() { return ((Host & UserMode) == Host); }
        public bool IsVoice() { return ((Voice & UserMode) == Voice); }
        public bool IsNormal() { return ((Normal | UserMode) == Normal); }

        public void SetAdmin(bool flag)
        {
            if (flag) { if (!IsAdmin()) { UserMode += Admin; } }
            else { if (IsAdmin()) { UserMode -= Admin; } }
        }

        public void SetOwner(bool flag)
        {
            if (flag) { if (!IsOwner()) { UserMode += Owner; } }
            else { if (IsOwner()) { UserMode -= Owner; } }
            UpdateFlag();
        }

        public void SetHost(bool flag)
        {
            if (flag) { if (!IsHost()) { UserMode += Host; } }
            else { if (IsHost()) { UserMode -= Host; } }
            UpdateFlag();
        }

        public void SetVoice(bool flag)
        {
            if (flag) { if (!IsVoice()) { UserMode += Voice; } }
            else { if (IsVoice()) { UserMode -= Voice; } }
            UpdateFlag();
        }
        public void UpdateFlag()
        {
            if (IsOwner()) { modeChar = Resources.ChannelUserFlagOwner; }
            else if (IsHost()) { modeChar = Resources.ChannelUserFlagHost; }
            else if (IsVoice()) { modeChar = Resources.ChannelUserFlagVoice; }
            else { modeChar = 0x0; }
        }
        public String8 ModeString
        {
            get
            {
                if (IsOwner()) { return Resources.FlagOwner; }
                else if (IsHost()) { return Resources.FlagHost; }
                else if (IsVoice()) { return Resources.FlagVoice; }
                else { return Resources.Null; }
            }
        }

        public void SetNormal()
        {
            UserMode = Normal;
            UpdateFlag();
        }


    };

    public class ModeCollection
    {
        public List<Mode> Modes;

        public ModeCollection()
        {
            Modes = new List<Mode>();
        }
        public Mode ResolveMode(byte ModeChar)
        {
            for (int i = 0; i < Modes.Count; i++) { if (Modes[i].ModeChar == ModeChar) { return Modes[i]; } }
            return null;
        }
    };


    public enum AuditModeType { S_OK, NEEDMOREPARAMS, UNKNOWNMODE, CHANOPRIVSNEEDED, KEYSET, USERSDONTMATCH, UMODEUNKNOWNFLAG };
    public class AuditMode
    {
        public byte modeChar;
        public bool modeFlag;
        public AuditModeType Audit;

        public AuditMode(byte modeChar, bool modeFlag) { this.modeChar = modeChar; this.modeFlag = modeFlag; }
    }
    public class AuditUserMode : AuditMode
    {
        public User TargetUser;
        public String8 user;
        public String8 modeData;
        public AuditUserMode(User TargetUser, String8 user, byte modeChar, bool modeFlag) : base(modeChar, modeFlag) { this.TargetUser = TargetUser; this.user = user; modeData = CreateModeData(modeFlag, modeChar, false); }

        public static String8 CreateModeData(bool modeFlag, byte modeChar, bool bSpacePos)
        {
            String8 ModeData = new String8(3);
            if (bSpacePos) { ModeData.append(0x20); }
            ModeData.append((modeFlag ? (byte)'+' : (byte)'-'));
            ModeData.append(modeChar);
            if (!bSpacePos) { ModeData.append(0x20); }
            return ModeData;
        }
    }
    public class AuditModeReport
    {
        public Int64 ModesModified; //each bit is a a-zA-Z back to back
        public List<AuditMode> Modes;
        public List<AuditUserMode> UserModes;

        public int Mode2Modifier(byte modeChar)
        {
            if ((modeChar >= (byte)'a') && (modeChar <= 'z')) { return modeChar - (byte)'a'; }
            else { return modeChar - (byte)'A' + 27; } //after the initial alphabet
        }

        public AuditModeReport()
        {
            Modes = new List<AuditMode>();
            UserModes = new List<AuditUserMode>();
        }
        public void SetModeFlagProcessed(byte modeChar)
        {
            if (!GetModeFlagProcessed(modeChar))
            {
                ModesModified += (1 << Mode2Modifier(modeChar));
            }
        }
        public bool GetModeFlagProcessed(byte modeChar)
        {
            int modeModifier = 1 << Mode2Modifier(modeChar);
            return ((ModesModified & modeModifier) == modeModifier);
        }
    }



    #region Server Modes

    #endregion

    #region User Modes
    public class UserModeAdmin : Mode { public UserModeAdmin() { this.ModeChar = Resources.UserModeCharAdmin; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new UserModeAdminFunction(); } }
    public class UserModeOper : Mode { public UserModeOper() { this.ModeChar = Resources.UserModeCharOper; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new UserModeOperFunction(); } }
    public class UserModeInvisible : Mode { public UserModeInvisible() { this.ModeChar = Resources.UserModeCharInvisible; this.Value = 0; this.Level = ChanUserMode.Normal; Function = new UserModeInvisibleFunction(); } }
    public class UserModeIrcx : Mode { public UserModeIrcx() { this.ModeChar = Resources.UserModeCharIrcx; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new UserModeIrcxFunction(); } }
    public class UserModeGag : Mode { public UserModeGag() { this.ModeChar = Resources.UserModeCharGag; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new UserModeGagFunction(); } }
    public class UserModePasskey : Mode { public UserModePasskey() { this.ModeChar = Resources.UserModePasskey; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new UserModePasskeyFunction(); } }
    public class UserModeSecure : Mode { public UserModeSecure() { this.ModeChar = (byte)'s'; this.Value = 0; this.Level = ChanUserMode.Admin; Function = null;  } }

    public class UserModeCollection : ModeCollection
    {
        String8 UserModes;

        public UserModeAdmin Admin = new UserModeAdmin();
        public UserModeOper Oper = new UserModeOper();
        public UserModeInvisible Invisible = new UserModeInvisible();
        public UserModeIrcx Ircx = new UserModeIrcx();
        public UserModeGag Gag = new UserModeGag();
        public UserModePasskey Passkey = new UserModePasskey();
        public UserModeSecure Secure = new UserModeSecure();

        public String8 UserModeString { get { return UserModes; } }

        public UserModeCollection()
            : base()
        {
            Modes.Add(Admin);
            Modes.Add(Oper);
            Modes.Add(Invisible);
            Modes.Add(Ircx);
            Modes.Add(Passkey);
            Modes.Add(Gag);
            Modes.Add(Secure);
            UserModes = new String8(Modes.Count + 2); //Modes, extra 1 for +, extra 1 for Ircx 'x'
            UpdateModes();
        }

        public void UpdateModes()
        {
            UserModes.length = 0;
            //UserModes.append('+');
            bool bHasLimit = false, bHasKey = false;
            String8 limit = Resources.Null;

            //UserModes.append(Ircx.ModeChar);
            for (int i = 0; i < Modes.Count; i++)
            {
                if (Modes[i].Value == 0x1) { UserModes.append(Modes[i].ModeChar); }
            }
        }
    }

    #endregion

    #region Channel Modes
    public class ChannelModeAuthOnly : Mode { public ChannelModeAuthOnly() { this.ModeChar = Resources.ChannelModeCharAuthOnly; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new ChannelModeAuthOnlyFunction(); } }
    // class ChannelModeExpiry : Mode { public ChannelModeExpiry() { this.ModeChar = Resources.ChannelModeCharExpiry; this.Value = 0; this.Level = UserAccessLevel.ChatGuide; } }
    public class ChannelModeProfanity : Mode { public ChannelModeProfanity() { this.ModeChar = Resources.ChannelModeCharProfanity; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new ChannelModeProfanityFunction(); } }
    public class ChannelModeOnStage : Mode { public ChannelModeOnStage() { this.ModeChar = Resources.ChannelModeCharOnStage; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new ChannelModeOnStageFunction(); } }
    public class ChannelModeHidden : Mode
    {
        public ChannelModeHidden()
        {
            this.ModeChar = Resources.ChannelModeCharHidden;
            this.Value = 0;
            this.Level = ChanUserMode.Host;
            //Function = new ChannelModeHiddenFunction();
        }
    }
    public class ChannelModeKey : Mode { public ChannelModeKey() { this.ModeChar = Resources.ChannelModeCharKey; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeKeyFunction(); } }
    public class ChannelModeInvite : Mode { public ChannelModeInvite() { this.ModeChar = Resources.ChannelModeCharInvite; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeInviteFunction(); } }
    public class ChannelModeUserLimit : Mode { public ChannelModeUserLimit() { this.ModeChar = Resources.ChannelModeCharUserLimit; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeUserLimitFunction(); } }
    public class ChannelModeModerated : Mode { public ChannelModeModerated() { this.ModeChar = Resources.ChannelModeCharModerated; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeModeratedFunction(); } }
    public class ChannelModeNoExtern : Mode { public ChannelModeNoExtern() { this.ModeChar = Resources.ChannelModeCharNoExtern; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeNoExternFunction(); } }
    public class ChannelModePrivate : Mode { public ChannelModePrivate() { this.ModeChar = Resources.ChannelModeCharPrivate; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModePrivateFunction(); } }
    public class ChannelModeRegistered : Mode { public ChannelModeRegistered() { this.ModeChar = Resources.ChannelModeCharRegistered; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new ChannelModeRegisteredFunction(); } }
    public class ChannelModeSecret : Mode { public ChannelModeSecret() { this.ModeChar = Resources.ChannelModeCharSecret; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeSecretFunction(); } }
    public class ChannelModeSubscriber : Mode { public ChannelModeSubscriber() { this.ModeChar = Resources.ChannelModeCharSubscriber; this.Value = 0; this.Level = ChanUserMode.Admin; Function = new ChannelModeSubscriberFunction(); } }
    public class ChannelModeTopicOp : Mode { public ChannelModeTopicOp() { this.ModeChar = Resources.ChannelModeCharTopicOp; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeTopicOpFunction(); } }
    public class ChannelModeKnock : Mode { public ChannelModeKnock() { this.ModeChar = Resources.ChannelModeCharKnock; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeKnockFunction(); } }
    public class ChannelModeNoWhisper : Mode { public ChannelModeNoWhisper() { this.ModeChar = Resources.ChannelModeCharNoWhisper; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeNoWhisperFunction(); } }
    public class ChannelModeNoGuestWhisper : Mode { public ChannelModeNoGuestWhisper() { this.ModeChar = Resources.ChannelModeCharNoGuestWhisper; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeNoGuestWhisperFunction(); } }
    //Note that auditorium mode may only be set at channel creation time using the CREATE command.
    public class ChannelModeAuditorium : Mode { public ChannelModeAuditorium() { this.ModeChar = Resources.ChannelModeCharAuditorium; this.Value = 0; this.Level = uint.MaxValue; Function = new ChannelModeAuditoriumFunction(); } }
    public class ChannelUserModeBan : Mode { public ChannelUserModeBan() { this.ModeChar = Resources.ChannelUserModeCharBan; this.Value = 0; this.Level = ChanUserMode.Host; Function = new ChannelModeBanFunction(); } }

    public class ChannelUserModeOwner : Mode { public ChannelUserModeOwner() { this.ModeChar = Resources.ChannelUserModeCharOwner; this.Value = 0; this.Level = ChanUserMode.Owner; } }
    public class ChannelUserModeHost : Mode { public ChannelUserModeHost() { this.ModeChar = Resources.ChannelUserModeCharHost; this.Value = 0; this.Level = ChanUserMode.Host; } }
    public class ChannelUserModeVoice : Mode { public ChannelUserModeVoice() { this.ModeChar = Resources.ChannelUserModeCharVoice; this.Value = 0; this.Level = ChanUserMode.Host; } }

    public class ChannelModeCollection : ModeCollection
    {
        String8 ChanModes;
        int modeLen;
        int modeFullLen;

        public ChannelModeAuthOnly AuthOnly = new ChannelModeAuthOnly();
        //   public ChannelModeExpiry Expiry = new ChannelModeExpiry();
        public ChannelModeProfanity Profanity = new ChannelModeProfanity();
        public ChannelModeOnStage OnStage = new ChannelModeOnStage();
        public ChannelModeHidden Hidden = new ChannelModeHidden();
        public ChannelModeKey Key = new ChannelModeKey();
        public ChannelModeInvite Invite = new ChannelModeInvite();
        public ChannelModeUserLimit UserLimit = new ChannelModeUserLimit();
        public ChannelModeModerated Moderated = new ChannelModeModerated();
        public ChannelModeNoExtern NoExtern = new ChannelModeNoExtern();
        public ChannelModePrivate Private = new ChannelModePrivate();
        public ChannelModeRegistered Registered = new ChannelModeRegistered();
        public ChannelModeSecret Secret = new ChannelModeSecret();
        public ChannelModeSubscriber Subscriber = new ChannelModeSubscriber();
        public ChannelModeTopicOp TopicOp = new ChannelModeTopicOp();
        public ChannelModeKnock Knock = new ChannelModeKnock();
        public ChannelModeNoWhisper NoWhisper = new ChannelModeNoWhisper();
        public ChannelModeNoGuestWhisper NoGuestWhisper = new ChannelModeNoGuestWhisper();
        public ChannelModeAuditorium Auditorium = new ChannelModeAuditorium();

        public ChannelUserModeOwner Owner = new ChannelUserModeOwner();
        public ChannelUserModeHost Host = new ChannelUserModeHost();
        public ChannelUserModeVoice Voice = new ChannelUserModeVoice();
        public ChannelUserModeBan Ban = new ChannelUserModeBan();

        public String8 ChannelModeString { get { ChanModes.length = modeFullLen; return ChanModes; } }
        public String8 ChannelModeShortString { get { ChanModes.length = modeLen; return ChanModes; } }



        public ChannelModeCollection()
            : base()
        {
            Modes.Add(Owner);
            Modes.Add(Host);
            Modes.Add(Voice);
            Modes.Add(AuthOnly);
            //     Modes.Add(Expiry);
            Modes.Add(Profanity);
            Modes.Add(OnStage);
            Modes.Add(Hidden);
            Modes.Add(Key);
            Modes.Add(Invite);
            Modes.Add(UserLimit);
            Modes.Add(Moderated);
            Modes.Add(NoExtern);
            Modes.Add(Private);
            Modes.Add(Registered);
            Modes.Add(Secret);
            Modes.Add(Subscriber);
            Modes.Add(TopicOp);
            Modes.Add(Knock);
            Modes.Add(NoWhisper);
            Modes.Add(NoGuestWhisper);
            Modes.Add(Auditorium);
            Modes.Add(Ban);
            //It might be worth considering having an array of 26a-z 26A-Z + other space and the modes not set will be ignored in raw processing '\0' then dont output
            ChanModes = new String8(Modes.Count + 2 + 10 + 31); //Modes, 2 ' ', 10 Limit, 31 Key
            NoExtern.Value = 1;
            TopicOp.Value = 1;
            UserLimit.Value = 50;
            UpdateModes(null);
        }
        public void UpdateModes(Prop key)
        {
            ChanModes.length = 0;
            ChanModes.append('+');
            bool bHasLimit = false, bHasKey = false;
            String8 limit = Resources.Null;

            for (int i = 0; i < Modes.Count; i++)
            {
                if (Modes[i].ModeChar == (byte)'l') { if (Modes[i].Value != 0) { bHasLimit = true; limit = new String8(Modes[i].Value.ToString()); } }
                else if (Modes[i].ModeChar == (byte)'k') { if (Modes[i].Value != 0) { bHasKey = true; } }
                else { if (Modes[i].Value == 0x1) { ChanModes.append(Modes[i].ModeChar); } }
            }
            if (bHasLimit) { ChanModes.append((byte)'l'); }
            if (bHasKey) { ChanModes.append((byte)'k'); }

            modeLen = ChanModes.length;

            if (bHasLimit) { ChanModes.append(' '); ChanModes.append(limit); }
            if (key != null)
            {
                if (bHasKey) { ChanModes.append(' '); ChanModes.append(key.Value); }
            }

            modeFullLen = ChanModes.length;
        }

    };
    #endregion

}
