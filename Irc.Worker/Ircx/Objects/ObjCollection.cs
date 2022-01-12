using System.Collections.Generic;
using System.Globalization;
using Irc.ClassExtensions.CSharpTools;

namespace Irc.Worker.Ircx.Objects;

public class ObjCollection
{
    private ObjType ObjectType;

    public ObjCollection(ObjType ObjectType)
    {
        this.ObjectType = ObjectType;
    }

    public List<Obj> ObjectCollection { get; } = new();

    public int Length => ObjectCollection.Count;

    public void Add(Obj obj)
    {
        ObjectCollection.Add(obj);
    }

    public void Remove(Obj obj)
    {
        ObjectCollection.Remove(obj);
    }

    public void Clear()
    {
        ObjectCollection.Clear();
    }

    public Obj IndexOf(int i)
    {
        return ObjectCollection[i];
    }

    public Obj FindObj(string Name, ObjIdentifier ObjType)
    {
        switch (ObjType)
        {
            case ObjIdentifier.ObjIdInternal:
            {
                // Search as OID
                return FindObjByOID(Name);
            }
            case ObjIdentifier.ObjIdIRCUserHex:
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

    public Obj FindObjByOID(string OID)
    {
        long oid;
        long.TryParse(OID, NumberStyles.HexNumber, null, out oid);

        for (var c = 0; c < ObjectCollection.Count; c++)
            if (ObjectCollection[c].OID == oid)
                return ObjectCollection[c];
        return null;
    }

    public Obj FindObjByHex(string Hex)
    {
        var HexString = new string(Hex.Substring(1));

        HexString = Tools.HexToString(HexString);

        return FindObjByName(HexString);
    }

    public Obj FindObjByName(string Name)
    {
        for (var c = 0; c < ObjectCollection.Count; c++)
            if (ObjectCollection[c].Name.ToUpper() == Name.ToUpper())
                return ObjectCollection[c];
        return null;
    }
}