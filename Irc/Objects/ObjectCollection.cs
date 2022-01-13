using System.Collections.Generic;
using Irc.ClassExtensions.CSharpTools;
using Irc.Objects;

namespace Irc.Worker.Ircx.Objects;

public class ObjectCollection<T> : IObjectCollection<T> where T : ChatObject
{
    public List<T> ChatObjects { get; } = new();

    public int Length => ChatObjects.Count;

    public void Add(T chatObject)
    {
        ChatObjects.Add(chatObject);
    }

    public void Remove(T chatObject)
    {
        ChatObjects.Remove(chatObject);
    }

    public void Clear()
    {
        ChatObjects.Clear();
    }

    public T IndexOf(int i)
    {
        return ChatObjects[i];
    }

    public T FindObj(string Name, IrcHelper.ObjIdentifier objectType)
    {
        switch (objectType)
        {
            case IrcHelper.ObjIdentifier.ObjIdInternal:
            {
                // Search as OID
                return FindObjByOID(Name);
            }
            case IrcHelper.ObjIdentifier.ObjIdIRCUserHex:
            {
                // Search as HEX
                return FindObjByHex(Name);
            }
            default:
            {
                // Search by Name
                return FindObjByName(Name);
            }
        }
    }

    public T FindObjByOID(string objectId)
    {
        foreach (var obj in ChatObjects)
            if (obj.Id.ToString() == objectId)
                return obj;

        return default(T);
    }

    public T FindObjByHex(string Hex)
    {
        var HexString = new string(Hex.Substring(1));

        HexString = Tools.HexToString(HexString);

        return FindObjByName(HexString);
    }

    public T FindObjByName(string Name)
    {
        for (var c = 0; c < ChatObjects.Count; c++)
            if (ChatObjects[c].Name.ToUpper() == Name.ToUpper())
                return ChatObjects[c];
        return default(T);
    }
}