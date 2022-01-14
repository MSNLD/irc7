using System.Collections.Generic;
using System.Text;
using Irc.Constants;
using Irc.Worker.Ircx;
using Irc.Worker.Ircx.Commands;
using Irc.Worker.Ircx.Objects;

/*You must specify the protocol as IRC3 if you want 
to use the additional MODE flag of +h (see below).
 * 
 * Obviously make an exception for IRC0 (mIRC compliant) and IRC9 (MSN compliant) experimental ircvers
 * Actually this means that IRC1-2 cannot use it, which will be funny
*/

namespace Irc.Worker;

public abstract class ModeFunction
{
}

public abstract class ChannelModeFunction : ModeFunction
{
    public abstract bool Execute(Server server, Channel channel, User user, bool bModeFlag, ref AuditModeReport report,
        Message message);
}

public abstract class UserModeFunction : ModeFunction
{
    public abstract bool Execute(Server server, User user, User TargetUser, bool bModeFlag, ref AuditModeReport report,
        Message message);
}

public class Mode
{
    public ModeFunction Function;
    public uint Level;
    public byte ModeChar;
    public int Value;

    public Mode(byte ModeChar, int Value, uint Level, ModeFunction Function)
    {
        this.ModeChar = ModeChar;
        this.Value = Value;
        this.Level = Level;
        this.Function = Function;
    }

    public Mode()
    {
    }
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

    public string ModeString
    {
        get
        {
            if (IsOwner())
                return Resources.FlagOwner;
            if (IsHost())
                return Resources.FlagHost;
            if (IsVoice())
                return Resources.FlagVoice;
            return string.Empty;
        }
    }

    public bool IsAdmin()
    {
        return (Admin & UserMode) == Admin;
    }

    public bool IsOwner()
    {
        return (Owner & UserMode) == Owner;
    }

    public bool IsHost()
    {
        return (Host & UserMode) == Host;
    }

    public bool IsVoice()
    {
        return (Voice & UserMode) == Voice;
    }

    public bool IsNormal()
    {
        return (Normal | UserMode) == Normal;
    }

    public void SetAdmin(bool flag)
    {
        if (flag)
        {
            if (!IsAdmin()) UserMode += Admin;
        }
        else
        {
            if (IsAdmin()) UserMode -= Admin;
        }
    }

    public void SetOwner(bool flag)
    {
        if (flag)
        {
            if (!IsOwner()) UserMode += Owner;
        }
        else
        {
            if (IsOwner()) UserMode -= Owner;
        }

        UpdateFlag();
    }

    public void SetHost(bool flag)
    {
        if (flag)
        {
            if (!IsHost()) UserMode += Host;
        }
        else
        {
            if (IsHost()) UserMode -= Host;
        }

        UpdateFlag();
    }

    public void SetVoice(bool flag)
    {
        if (flag)
        {
            if (!IsVoice()) UserMode += Voice;
        }
        else
        {
            if (IsVoice()) UserMode -= Voice;
        }

        UpdateFlag();
    }

    public void UpdateFlag()
    {
        if (IsOwner())
            modeChar = Resources.ChannelUserFlagOwner;
        else if (IsHost())
            modeChar = Resources.ChannelUserFlagHost;
        else if (IsVoice())
            modeChar = Resources.ChannelUserFlagVoice;
        else
            modeChar = 0x0;
    }

    public void SetNormal()
    {
        UserMode = Normal;
        UpdateFlag();
    }
}

public class ModeCollection
{
    public List<Mode> Modes;

    public ModeCollection()
    {
        Modes = new List<Mode>();
    }

    public Mode ResolveMode(byte ModeChar)
    {
        for (var i = 0; i < Modes.Count; i++)
            if (Modes[i].ModeChar == ModeChar)
                return Modes[i];
        return null;
    }
}

public enum AuditModeType
{
    S_OK,
    NEEDMOREPARAMS,
    UNKNOWNMODE,
    CHANOPRIVSNEEDED,
    KEYSET,
    USERSDONTMATCH,
    UMODEUNKNOWNFLAG
}

public class AuditMode
{
    public AuditModeType Audit;
    public byte modeChar;
    public bool modeFlag;

    public AuditMode(byte modeChar, bool modeFlag)
    {
        this.modeChar = modeChar;
        this.modeFlag = modeFlag;
    }
}

public class AuditUserMode : AuditMode
{
    public string modeData;
    public User TargetUser;
    public string user;

    public AuditUserMode(User TargetUser, string user, byte modeChar, bool modeFlag) : base(modeChar, modeFlag)
    {
        this.TargetUser = TargetUser;
        this.user = user;
        modeData = CreateModeData(modeFlag, modeChar, false);
    }

    public static string CreateModeData(bool modeFlag, byte modeChar, bool bSpacePos)
    {
        var ModeData = new StringBuilder(3);
        if (bSpacePos) ModeData.Append((char) 0x20);
        ModeData.Append(modeFlag ? '+' : '-');
        ModeData.Append((char) modeChar);
        if (!bSpacePos) ModeData.Append((char) 0x20);
        return ModeData.ToString();
    }
}

public class AuditModeReport
{
    public List<AuditMode> Modes;
    public long ModesModified; //each bit is a a-zA-Z back to back
    public List<AuditUserMode> UserModes;

    public AuditModeReport()
    {
        Modes = new List<AuditMode>();
        UserModes = new List<AuditUserMode>();
    }

    public int Mode2Modifier(byte modeChar)
    {
        if (modeChar >= (byte) 'a' && modeChar <= 'z')
            return modeChar - (byte) 'a';
        return modeChar - (byte) 'A' + 27;
    }

    public void SetModeFlagProcessed(byte modeChar)
    {
        if (!GetModeFlagProcessed(modeChar)) ModesModified += 1 << Mode2Modifier(modeChar);
    }

    public bool GetModeFlagProcessed(byte modeChar)
    {
        var modeModifier = 1 << Mode2Modifier(modeChar);
        return (ModesModified & modeModifier) == modeModifier;
    }
}

#region Server Modes

#endregion

#region User Modes

public class UserModeAdmin : Mode
{
    public UserModeAdmin()
    {
        ModeChar = Resources.UserModeCharAdmin;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new UserModeAdminFunction();
    }
}

public class UserModeOper : Mode
{
    public UserModeOper()
    {
        ModeChar = Resources.UserModeCharOper;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new UserModeOperFunction();
    }
}

public class UserModeInvisible : Mode
{
    public UserModeInvisible()
    {
        ModeChar = Resources.UserModeCharInvisible;
        Value = 0;
        Level = ChanUserMode.Normal;
        Function = new UserModeInvisibleFunction();
    }
}

public class UserModeIrcx : Mode
{
    public UserModeIrcx()
    {
        ModeChar = Resources.UserModeCharIrcx;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new UserModeIrcxFunction();
    }
}

public class UserModeGag : Mode
{
    public UserModeGag()
    {
        ModeChar = Resources.UserModeCharGag;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new UserModeGagFunction();
    }
}

public class UserModePasskey : Mode
{
    public UserModePasskey()
    {
        ModeChar = Resources.UserModePasskey;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new UserModePasskeyFunction();
    }
}

public class UserModeSecure : Mode
{
    public UserModeSecure()
    {
        ModeChar = (byte) 's';
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = null;
    }
}

public class UserModeCollection : ModeCollection
{
    public UserModeAdmin Admin = new();
    public UserModeGag Gag = new();
    public UserModeInvisible Invisible = new();
    public UserModeIrcx Ircx = new();
    public UserModeOper Oper = new();
    public UserModePasskey Passkey = new();
    public UserModeSecure Secure = new();
    private readonly StringBuilder UserModes;

    public UserModeCollection()
    {
        Modes.Add(Admin);
        Modes.Add(Oper);
        Modes.Add(Invisible);
        Modes.Add(Ircx);
        Modes.Add(Passkey);
        Modes.Add(Gag);
        Modes.Add(Secure);
        UserModes = new StringBuilder(Modes.Count + 2); //Modes, extra 1 for +, extra 1 for Ircx 'x'
        UpdateModes();
    }

    public string UserModeString => UserModes.ToString();

    public void UpdateModes()
    {
        UserModes.Length = 0;
        //UserModes.Append('+');
        bool bHasLimit = false, bHasKey = false;
        var limit = string.Empty;

        //UserModes.Append(Ircx.ModeChar);
        for (var i = 0; i < Modes.Count; i++)
            if (Modes[i].Value == 0x1)
                UserModes.Append((char) Modes[i].ModeChar);
    }
}

#endregion

#region Channel Modes

public class ChannelModeAuthOnly : Mode
{
    public ChannelModeAuthOnly()
    {
        ModeChar = Resources.ChannelModeCharAuthOnly;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new ChannelModeAuthOnlyFunction();
    }
}

// class ChannelModeExpiry : Mode { public ChannelModeExpiry() { this.ModeChar = Resources.ChannelModeCharExpiry; this.Value = 0; this.Level = UserAccessLevel.ChatGuide; } }
public class ChannelModeProfanity : Mode
{
    public ChannelModeProfanity()
    {
        ModeChar = Resources.ChannelModeCharProfanity;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new ChannelModeProfanityFunction();
    }
}

public class ChannelModeOnStage : Mode
{
    public ChannelModeOnStage()
    {
        ModeChar = Resources.ChannelModeCharOnStage;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new ChannelModeOnStageFunction();
    }
}

public class ChannelModeHidden : Mode
{
    public ChannelModeHidden()
    {
        ModeChar = Resources.ChannelModeCharHidden;
        Value = 0;
        Level = ChanUserMode.Host;
        //Function = new ChannelModeHiddenFunction();
    }
}

public class ChannelModeKey : Mode
{
    public ChannelModeKey()
    {
        ModeChar = Resources.ChannelModeCharKey;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeKeyFunction();
    }
}

public class ChannelModeInvite : Mode
{
    public ChannelModeInvite()
    {
        ModeChar = Resources.ChannelModeCharInvite;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeInviteFunction();
    }
}

public class ChannelModeUserLimit : Mode
{
    public ChannelModeUserLimit()
    {
        ModeChar = Resources.ChannelModeCharUserLimit;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeUserLimitFunction();
    }
}

public class ChannelModeModerated : Mode
{
    public ChannelModeModerated()
    {
        ModeChar = Resources.ChannelModeCharModerated;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeModeratedFunction();
    }
}

public class ChannelModeNoExtern : Mode
{
    public ChannelModeNoExtern()
    {
        ModeChar = Resources.ChannelModeCharNoExtern;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeNoExternFunction();
    }
}

public class ChannelModePrivate : Mode
{
    public ChannelModePrivate()
    {
        ModeChar = Resources.ChannelModeCharPrivate;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModePrivateFunction();
    }
}

public class ChannelModeRegistered : Mode
{
    public ChannelModeRegistered()
    {
        ModeChar = Resources.ChannelModeCharRegistered;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new ChannelModeRegisteredFunction();
    }
}

public class ChannelModeSecret : Mode
{
    public ChannelModeSecret()
    {
        ModeChar = Resources.ChannelModeCharSecret;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeSecretFunction();
    }
}

public class ChannelModeSubscriber : Mode
{
    public ChannelModeSubscriber()
    {
        ModeChar = Resources.ChannelModeCharSubscriber;
        Value = 0;
        Level = ChanUserMode.Admin;
        Function = new ChannelModeSubscriberFunction();
    }
}

public class ChannelModeTopicOp : Mode
{
    public ChannelModeTopicOp()
    {
        ModeChar = Resources.ChannelModeCharTopicOp;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeTopicOpFunction();
    }
}

public class ChannelModeKnock : Mode
{
    public ChannelModeKnock()
    {
        ModeChar = Resources.ChannelModeCharKnock;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeKnockFunction();
    }
}

public class ChannelModeNoWhisper : Mode
{
    public ChannelModeNoWhisper()
    {
        ModeChar = Resources.ChannelModeCharNoWhisper;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeNoWhisperFunction();
    }
}

public class ChannelModeNoGuestWhisper : Mode
{
    public ChannelModeNoGuestWhisper()
    {
        ModeChar = Resources.ChannelModeCharNoGuestWhisper;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeNoGuestWhisperFunction();
    }
}

//Note that auditorium mode may only be set at channel creation time using the CREATE command.
public class ChannelModeAuditorium : Mode
{
    public ChannelModeAuditorium()
    {
        ModeChar = Resources.ChannelModeCharAuditorium;
        Value = 0;
        Level = uint.MaxValue;
        Function = new ChannelModeAuditoriumFunction();
    }
}

public class ChannelUserModeBan : Mode
{
    public ChannelUserModeBan()
    {
        ModeChar = Resources.ChannelUserModeCharBan;
        Value = 0;
        Level = ChanUserMode.Host;
        Function = new ChannelModeBanFunction();
    }
}

public class ChannelUserModeOwner : Mode
{
    public ChannelUserModeOwner()
    {
        ModeChar = Resources.ChannelUserModeCharOwner;
        Value = 0;
        Level = ChanUserMode.Owner;
    }
}

public class ChannelUserModeHost : Mode
{
    public ChannelUserModeHost()
    {
        ModeChar = Resources.ChannelUserModeCharHost;
        Value = 0;
        Level = ChanUserMode.Host;
    }
}

public class ChannelUserModeVoice : Mode
{
    public ChannelUserModeVoice()
    {
        ModeChar = Resources.ChannelUserModeCharVoice;
        Value = 0;
        Level = ChanUserMode.Host;
    }
}

public class ChannelModeCollection : ModeCollection
{
    public ChannelModeAuditorium Auditorium = new();

    public ChannelModeAuthOnly AuthOnly = new();
    public ChannelUserModeBan Ban = new();
    private readonly StringBuilder ChanModes;
    public ChannelModeHidden Hidden = new();
    public ChannelUserModeHost Host = new();
    public ChannelModeInvite Invite = new();
    public ChannelModeKey Key = new();
    public ChannelModeKnock Knock = new();
    private int modeFullLen;
    private int modeLen;
    public ChannelModeModerated Moderated = new();
    public ChannelModeNoExtern NoExtern = new();
    public ChannelModeNoGuestWhisper NoGuestWhisper = new();
    public ChannelModeNoWhisper NoWhisper = new();
    public ChannelModeOnStage OnStage = new();

    public ChannelUserModeOwner Owner = new();

    public ChannelModePrivate Private = new();

    //   public ChannelModeExpiry Expiry = new ChannelModeExpiry();
    public ChannelModeProfanity Profanity = new();
    public ChannelModeRegistered Registered = new();
    public ChannelModeSecret Secret = new();
    public ChannelModeSubscriber Subscriber = new();
    public ChannelModeTopicOp TopicOp = new();
    public ChannelModeUserLimit UserLimit = new();
    public ChannelUserModeVoice Voice = new();


    public ChannelModeCollection()
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
        ChanModes = new StringBuilder(Modes.Count + 2 + 10 + 31); //Modes, 2 ' ', 10 Limit, 31 Key
        NoExtern.Value = 1;
        TopicOp.Value = 1;
        UserLimit.Value = 50;
        UpdateModes(null);
    }

    public string ChannelModeString
    {
        get
        {
            ChanModes.Length = modeFullLen;
            return ChanModes.ToString();
        }
    }

    public string ChannelModeShortString
    {
        get
        {
            ChanModes.Length = modeLen;
            return ChanModes.ToString();
        }
    }

    public void UpdateModes(string modeKey)
    {
        ChanModes.Length = 0;
        ChanModes.Append('+');
        bool bHasLimit = false, bHasKey = false;
        var limit = string.Empty;

        for (var i = 0; i < Modes.Count; i++)
            if (Modes[i].ModeChar == (byte) 'l')
            {
                if (Modes[i].Value != 0)
                {
                    bHasLimit = true;
                    limit = new string(Modes[i].Value.ToString());
                }
            }
            else if (Modes[i].ModeChar == (byte) 'k')
            {
                if (Modes[i].Value != 0) bHasKey = true;
            }
            else
            {
                if (Modes[i].Value == 0x1) ChanModes.Append((char) Modes[i].ModeChar);
            }

        if (bHasLimit) ChanModes.Append('l');
        if (bHasKey) ChanModes.Append('k');

        modeLen = ChanModes.Length;

        if (bHasLimit)
        {
            ChanModes.Append(' ');
            ChanModes.Append(limit);
        }

        if (modeKey != null)
            if (bHasKey)
            {
                ChanModes.Append(' ');
                ChanModes.Append(modeKey);
            }

        modeFullLen = ChanModes.Length;
    }
}

#endregion