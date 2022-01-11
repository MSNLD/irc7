using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;

namespace Core.Ircx.Objects
{
    public enum UserAccessLevel { None = 0, ChatGuest = 1, ChatUser = 2, ChatMember = 3, ChatHost = 4, ChatOwner = 5, ChatGuide = 6, ChatSysop = 7, ChatSysopManager = 8, ChatAdministrator = 9, NoAccess = 10, ChatService = 11 }
    public enum AccessResultEnum { SUCCESS_OWNER = -6, SUCCESS_HOST = -5, SUCCESS_VOICE = -4, SUCCESS_GRANTED = -3, SUCCESS_MEMBERKEY = -2, SUCCESS = -1, NONE = 0, ERR_ALREADYINCHANNEL = 1, ERR_NOSUCHNICK = 401, ERR_NICKINUSE = 433, ERR_CHANNELISFULL = 471, ERR_INVITEONLYCHAN = 473, ERR_BANNEDFROMCHAN = 474, ERR_BADCHANNELKEY = 475, ERR_AUTHONLYCHAN = 904 }

    public enum EnumAccessLevel { NONE = -2, DENY = -1, GRANT = 0, VOICE = 1, HOST = 2, OWNER = 3, All = 4 };
    public enum EnumAccessOperator { NONE = -1, ADD = 0, DELETE = 2, LIST = 3, CLEAR = 4 };

    public class AccessLevel
    {
        public EnumAccessLevel Level;
        public string LevelText;
        public static AccessLevel None = new AccessLevel(EnumAccessLevel.NONE, Resources.Null);

        public AccessLevel(EnumAccessLevel Level, string LevelText)
        {
            this.Level = Level;
            this.LevelText = LevelText;
        }
    }
    public class AccessOperator
    {
        public EnumAccessOperator Operator;
        public string OperatorText;
        public AccessOperator(EnumAccessOperator Operator, string OperatorText)
        {
            this.Operator = Operator;
            this.OperatorText = OperatorText;
        }
    }

    public class AccessLevelCollection
    {
        public List<AccessLevel> Levels;
        public AccessLevelCollection(bool IsChannel)
        {
            Levels = new List<AccessLevel>();
            if (IsChannel)
            {
                Levels.Add(new AccessLevel(EnumAccessLevel.OWNER, Resources.AccessLevelOwner));
                Levels.Add(new AccessLevel(EnumAccessLevel.HOST, Resources.AccessLevelHost));
                Levels.Add(new AccessLevel(EnumAccessLevel.VOICE, Resources.AccessLevelVoice));
            }
            Levels.Add(new AccessLevel(EnumAccessLevel.GRANT, Resources.AccessLevelGrant));
            Levels.Add(new AccessLevel(EnumAccessLevel.DENY, Resources.AccessLevelDeny));
        }
    }
    public class AccessOperatorCollection
    {
        public List<AccessOperator> Operators;
        public AccessOperatorCollection()
        {
            Operators = new List<AccessOperator>();
            Operators.Add(new AccessOperator(EnumAccessOperator.ADD, Resources.AccessEntryOperatorAdd));
            Operators.Add(new AccessOperator(EnumAccessOperator.DELETE, Resources.AccessEntryOperatorDelete));
            Operators.Add(new AccessOperator(EnumAccessOperator.LIST, Resources.AccessEntryOperatorList));
            Operators.Add(new AccessOperator(EnumAccessOperator.CLEAR, Resources.AccessEntryOperatorClear));
        }
    }

    public class AccessEntry
    {
        public AccessLevel Level;
        public Address Mask;
        public string EntryAddress;
        public UserAccessLevel EntryLevel;
        public int Duration;
        public string Reason;
        public bool Fixed;

        public int DurationInSeconds
        {
            get
            {
                return (int)Math.Ceiling((double)Duration / 60);
            }
        }
    };
    public class AccessCollection
    {
        public List<AccessEntry> Entries;
        public AccessCollection()
        {
            Entries = new List<AccessEntry>();
        }
        public AccessEntry Contains(Address QueryMask)
        {
            for (int i = 0; i < Entries.Count; i++)
            {
                if (QueryMask._address[3] == Entries[i].Mask._address[3]) { return Entries[i]; } // 3 = full address
            }
            return null;
        }
        public void Add(AccessEntry Entry) { Entries.Add(Entry); }
        public void Remove(AccessEntry Entry) { Entries.Remove(Entry); }
    };
    public class Access
    {
        public string ObjectName;
        public AccessCollection Entries;
        AccessLevelCollection LevelCollection;
        AccessOperatorCollection OperatorCollection;


        public Access(string ObjectName, bool IsChannel)
        {
            this.ObjectName = ObjectName;
            Entries = new AccessCollection();
            LevelCollection = new AccessLevelCollection(IsChannel);
            OperatorCollection = new AccessOperatorCollection();
        }

        public AccessLevel ResolveAccessLevel(string Data)
        {
            string Level = new string(Data.ToString().ToUpper());

            for (int i = 0; i < LevelCollection.Levels.Count; i++)
            {
                if (Level == LevelCollection.Levels[i].LevelText) { return LevelCollection.Levels[i]; }
            }

            return AccessLevel.None;
        }
        public EnumAccessOperator ResolveAccessOperator(string Data)
        {
            string Operator = new string(Data.ToString().ToUpper());

            for (int i = 0; i < OperatorCollection.Operators.Count; i++)
            {
                if (Operator == OperatorCollection.Operators[i].OperatorText) { return OperatorCollection.Operators[i].Operator; }
            }

            return EnumAccessOperator.NONE;
        }

        public enum AccessResultEnum
        {
            SUCCESS_OWNER = -6, SUCCESS_HOST = -5, SUCCESS_VOICE = -4, SUCCESS_GRANTED = -3, SUCCESS_MEMBERKEY = -2, SUCCESS = -1, NONE = 0, ERR_ALREADYINCHANNEL = 1, ERR_NOSUCHNICK = 401, ERR_NICKINUSE = 433, ERR_CHANNELISFULL = 471, ERR_INVITEONLYCHAN = 473, ERR_BANNEDFROMCHAN = 474, ERR_BADCHANNELKEY = 475, ERR_AUTHONLYCHAN = 904, ERR_SECUREONLYCHAN = 557
        }
        public class ObjectAccessResult
        {
            public AccessResultEnum Result;
            public AccessEntry Entry;
            public ObjectAccessResult()
            {
                Result = AccessResultEnum.NONE;
            }
        }

        public ObjectAccessResult GetAccess(Address Mask)
        {
            ObjectAccessResult AccessResult = new ObjectAccessResult();
            bool GrantExists = false;
            for (int i = 0; i < Entries.Entries.Count; i++)
            {
                if (Entries.Entries[i].Level.Level == EnumAccessLevel.GRANT) { GrantExists = true; }
                Address Entry = Entries.Entries[i].Mask;
                string TestAddress = (Entry.UsesIP ? Mask._address[4] : Mask._address[3]);
                if (StringBuilderRegEx.EvaluateString(Entry._address[3].ToString(), TestAddress.ToString(), true))
                {
                    AccessResultEnum EntryResult = AccessResultEnum.ERR_AUTHONLYCHAN;
                    switch (Entries.Entries[i].Level.Level)
                    {
                        case EnumAccessLevel.OWNER: { EntryResult = AccessResultEnum.SUCCESS_OWNER; break; }
                        case EnumAccessLevel.HOST: { EntryResult = AccessResultEnum.SUCCESS_HOST; break; }
                        case EnumAccessLevel.VOICE: { EntryResult = AccessResultEnum.SUCCESS_VOICE; break; }
                        case EnumAccessLevel.DENY: { EntryResult = AccessResultEnum.ERR_BANNEDFROMCHAN; break; }
                        case EnumAccessLevel.GRANT: { EntryResult = AccessResultEnum.SUCCESS_GRANTED; break; }
                    }
                    if (AccessResult.Result == AccessResultEnum.NONE) { AccessResult.Result = EntryResult; }
                    else if (EntryResult < AccessResult.Result) { AccessResult.Result = EntryResult; }

                    AccessResult.Entry = Entries.Entries[i];
                }
            }

            if ((GrantExists) && (AccessResult.Result >= AccessResultEnum.NONE))
            {
                AccessResult.Result = AccessResultEnum.ERR_BANNEDFROMCHAN; //GRANT works like an exclusive ban
            }
            return AccessResult;
        }
        public void Decrement()
        {
            List<AccessEntry> ExpiredEntries = new List<AccessEntry>();
            for (int i = 0; i < Entries.Entries.Count; i++)
            {
                AccessEntry ae = Entries.Entries[i];
                if (!ae.Fixed)
                {
                    if (--ae.Duration < 0) { ExpiredEntries.Add(ae); }
                }
            }

            for (int i = 0; i < ExpiredEntries.Count; i++)
            {
                Entries.Entries.Remove(ExpiredEntries[i]);
            }
        }
    };
}
