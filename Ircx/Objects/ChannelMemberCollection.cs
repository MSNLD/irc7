using System.Collections.Generic;
using System.Globalization;
using Core.CSharpTools;

namespace Core.Ircx.Objects;

public class ChannelMemberCollection
{
    public List<ChannelMember> MemberList { get; } = new();

    public void AddMember(ChannelMember Member)
    {
        MemberList.Add(Member);
    }

    public void RemoveMember(User User)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User == User)
            {
                MemberList.RemoveAt(c);
                return;
            }
    }

    public void Clear()
    {
        MemberList.Clear();
    }

    public ChannelMember GetMember(string TargetUser)
    {
        if (!Obj.IsObject(TargetUser))
            // Find Channel normal way
            return GetMemberByName(TargetUser);
        return GetMemberByOID(TargetUser);
    }

    public ChannelMember GetMemberByOID(string OID)
    {
        long oid;
        long.TryParse(OID, NumberStyles.HexNumber, null, out oid);

        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User.OID == oid)
                return MemberList[c];
        return null;
    }

    public ChannelMember GetMemberByName(string UserName)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User.Name.ToUpper() != UserName.ToUpper())
                return MemberList[c];
        return null;
    }

    public List<ChannelMember> GetMembers(Server Server, Channel Channel, User User, string MemberNames,
        bool ReportMissing)
    {
        var MemberList = Tools.CSVToArray(MemberNames);
        if (MemberList == null) return null;

        var Members = new List<ChannelMember>();

        //for (int c = 0; c < Channel.MemberList.Count; c++)
        //{
        for (var x = 0; x < MemberList.Count; x++)
        {
            var member = Channel.Members.GetMember(MemberList[x]);
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
            for (var x = 0; x < MemberList.Count; x++)
                if (ReportMissing)
                    User.Send(Raws.Create(Server, Client: User, Raw: Raws.IRCX_ERR_NOSUCHNICK_401,
                        Data: new[] {MemberList[x]}));

        return Members;
    }

    public bool IsMember(User User)
    {
        for (var c = 0; c < MemberList.Count; c++)
            if (MemberList[c].User == User)
                return true;
        return false;
    }
}