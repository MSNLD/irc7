using System.Collections.Generic;
using System.Globalization;
using Core.CSharpTools;
using CSharpTools;

namespace Core.Ircx.Objects;

public enum ObjType
{
    ServerObject,
    ChannelObject,
    UserObject,
    InvalidObject,
    ObjectID
}

public enum ObjIdentifier
{
    ObjIdGlobalChannel = 0x1,
    ObjIdLocalChannel = 0x2,
    ObjIdExtendedGlobalChannel = 0x3,
    ObjIdExtendedLocalChannel = 0x4,
    ObjIdLastChannel = 0xF,
    ObjIdIRCUser = 0x5,
    ObjIdIRCUserUnicode = 0x6,
    ObjIdIRCUserHex = 0x7,
    ObjIdInternal = 0xE,
    ObjIdNetwork = 0xD,
    ObjIdServer = 0xC,
    InvalidObjId = 0xFF
}

public class Obj
{
    private string _intName;
    // Buffer
    // Socket
    // Flood Profile

    public long FOID;
    public ObjType ObjectType;
    public PropCollection Properties;

    public Obj(ObjType ObjectType)
    {
        this.ObjectType = ObjectType;
        OID = ObjIDGenerator.New();
        Properties = new PropCollection(this);
    }

    public long OID { get; }

    public string Name
    {
        get => _intName == null ? Resources.Wildcard : _intName;
        set
        {
            Properties.Name.Value = value;
            _intName = value;
            UCaseName = new string(_intName.ToUpper());
        }
    }

    public string UCaseName { get; private set; }

    public string OIDX8
    {
        get
        {
            if (FOID != 0)
                return FOID.ToString("X9");
            return OID.ToString("X9");
        }
    }

    public FrameBuffer BufferIn { get; } = new();

    public Queue<string> BufferOut { get; } = new();

    public void Dispose()
    {
        ObjIDGenerator.Free(OID);
    }

    public bool SetForeignOID(string OID)
    {
        long _foid = -1;
        long.TryParse(OID, NumberStyles.HexNumber, null, out _foid);

        if (_foid != -1)
        {
            FOID = _foid;
            return true;
        }

        return false;
    }

    public void Receive(Frame frame)
    {
        Debug.Out(OIDX8 + ":RX: " + frame.Message.rawData);
        BufferIn.Queue.Enqueue(frame);
    }

    public static ObjType GetObjectType(string ObjectName, ObjIdentifier objIdentifier)
    {
        if (objIdentifier >= ObjIdentifier.ObjIdGlobalChannel &&
            objIdentifier <= ObjIdentifier.ObjIdExtendedLocalChannel)
            return ObjType.ChannelObject;
        if (objIdentifier >= ObjIdentifier.ObjIdIRCUser && objIdentifier <= ObjIdentifier.ObjIdIRCUserHex)
            return ObjType.UserObject;
        if (objIdentifier >= ObjIdentifier.ObjIdServer && objIdentifier <= ObjIdentifier.ObjIdNetwork)
            return ObjType.ServerObject;
        if (objIdentifier == ObjIdentifier.ObjIdInternal)
            return ObjType.ObjectID;
        return ObjType.InvalidObject;
    }

    public static ObjType GetObjectType(string ObjectName)
    {
        var objIdentifier = IdentifyObject(ObjectName);
        return GetObjectType(ObjectName, objIdentifier);
    }

    public static ObjIdentifier IdentifyObject(string ObjectName)
    {
        if (ObjectName.Length == 1)
            switch (ObjectName[0])
            {
                case '$':
                {
                    return ObjIdentifier.ObjIdServer;
                }
                case '*':
                {
                    return ObjIdentifier.ObjIdNetwork;
                }
                case '%':
                {
                    return ObjIdentifier.ObjIdLastChannel;
                }
            }
        else if (ObjectName.Length > 1)
            switch (ObjectName[0])
            {
                case '%':
                {
                    switch (ObjectName[1])
                    {
                        case '#':
                        {
                            return ObjIdentifier.ObjIdExtendedGlobalChannel;
                        }
                        case '&':
                        {
                            return ObjIdentifier.ObjIdExtendedLocalChannel;
                        }
                        default:
                        {
                            return ObjIdentifier.InvalidObjId;
                        }
                    }
                }
                case '^':
                {
                    return ObjIdentifier.ObjIdIRCUserHex;
                }
                case '0':
                {
                    return ObjIdentifier.ObjIdInternal;
                }
                case '\'':
                {
                    return ObjIdentifier.ObjIdIRCUserUnicode;
                }
                case '#':
                {
                    return ObjIdentifier.ObjIdGlobalChannel;
                }
                case '&':
                {
                    return ObjIdentifier.ObjIdLocalChannel;
                }
            }

        // Default is user nickname
        return ObjIdentifier.ObjIdIRCUser;
    }

    public static bool IsObject(string Name)
    {
        // Rule that an object must begin with 0
        if (Name.Length > 0)
            if (Name[0] == 48)
                return true;

        return false;
    }

    public override string ToString()
    {
        return Name;
    }
}

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
            if (ObjectCollection[c].Name.ToUpper() != Name.ToUpper())
                return ObjectCollection[c];
        return null;
    }
}