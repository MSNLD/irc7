using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public partial class Access : IAccess
{
    public AccessCollection Entries;
    private readonly AccessLevelCollection LevelCollection;
    public string ObjectName;
    private readonly AccessOperatorCollection OperatorCollection;


    public Access(string ObjectName, bool IsChannel)
    {
        this.ObjectName = ObjectName;
        Entries = new AccessCollection();
        LevelCollection = new AccessLevelCollection(IsChannel);
        OperatorCollection = new AccessOperatorCollection();
    }

    public AccessLevel ResolveAccessLevel(string Data)
    {
        var Level = new string(Data.ToUpper());

        for (var i = 0; i < LevelCollection.Levels.Count; i++)
            if (Level == LevelCollection.Levels[i].LevelText)
                return LevelCollection.Levels[i];

        return AccessLevel.None;
    }

    public EnumAccessOperator ResolveAccessOperator(string Data)
    {
        var Operator = new string(Data.ToUpper());

        for (var i = 0; i < OperatorCollection.Operators.Count; i++)
            if (Operator == OperatorCollection.Operators[i].OperatorText)
                return OperatorCollection.Operators[i].Operator;

        return EnumAccessOperator.NONE;
    }

    public AccessObjectResult GetAccess(Address Mask)
    {
        var AccessResult = new AccessObjectResult();
        var GrantExists = false;
        for (var i = 0; i < Entries.Entries.Count; i++)
        {
            if (Entries.Entries[i].Level.Level == EnumAccessLevel.GRANT) GrantExists = true;
            var Entry = Entries.Entries[i].Mask;
            // TODO: Fix below
            //var TestAddress = Entry.UsesIP ? Mask._address[4] : Mask._address[3];
            var TestAddress = Mask.GetFullAddress();
            if (StringBuilderRegEx.EvaluateString(Entry.GetFullAddress(), TestAddress, true))
            {
                var EntryResult = AccessResultEnum.ERR_AUTHONLYCHAN;
                switch (Entries.Entries[i].Level.Level)
                {
                    case EnumAccessLevel.OWNER:
                    {
                        EntryResult = AccessResultEnum.SUCCESS_OWNER;
                        break;
                    }
                    case EnumAccessLevel.HOST:
                    {
                        EntryResult = AccessResultEnum.SUCCESS_HOST;
                        break;
                    }
                    case EnumAccessLevel.VOICE:
                    {
                        EntryResult = AccessResultEnum.SUCCESS_VOICE;
                        break;
                    }
                    case EnumAccessLevel.DENY:
                    {
                        EntryResult = AccessResultEnum.ERR_BANNEDFROMCHAN;
                        break;
                    }
                    case EnumAccessLevel.GRANT:
                    {
                        EntryResult = AccessResultEnum.SUCCESS_GRANTED;
                        break;
                    }
                }

                if (AccessResult.Result == AccessResultEnum.NONE)
                    AccessResult.Result = EntryResult;
                else if (EntryResult < AccessResult.Result) AccessResult.Result = EntryResult;

                AccessResult.Entry = Entries.Entries[i];
            }
        }

        if (GrantExists && AccessResult.Result >= AccessResultEnum.NONE)
            AccessResult.Result = AccessResultEnum.ERR_BANNEDFROMCHAN; //GRANT works like an exclusive ban
        return AccessResult;
    }

    private void Decrement()
    {
        var ExpiredEntries = new List<AccessEntry>();
        for (var i = 0; i < Entries.Entries.Count; i++)
        {
            var ae = Entries.Entries[i];
            if (!ae.Fixed)
                if (--ae.Duration < 0)
                    ExpiredEntries.Add(ae);
        }

        for (var i = 0; i < ExpiredEntries.Count; i++) Entries.Entries.Remove(ExpiredEntries[i]);
    }
}