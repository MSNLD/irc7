using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpTools;

namespace Core.Ircx.Objects
{
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
        // Buffer
        // Socket
        // Flood Profile

        FrameBuffer bufferIn = new FrameBuffer();
        Queue<string> bufferOut = new Queue<string>();
        string _intName = null, _intUName = null;
        public long OID { get; }
        public long FOID;
        public ObjType ObjectType;
        public PropCollection Properties;

        public Obj(ObjType ObjectType)
        {
            this.ObjectType = ObjectType;
            OID = ObjIDGenerator.New();
            Properties = new PropCollection(this);
        }
        
        public void Dispose()
        {
            ObjIDGenerator.Free(OID);
        }

        public string Name {
            get {
                return (_intName == null ? Resources.Wildcard : _intName);
            }
            set {
                Properties.Name.Value = value;
                _intName = value;
                _intUName = new string(_intName.ToString().ToUpper());
            } }
        public string UCaseName { get { return _intUName; } }
        public string OIDX8 {
            get {
                if (FOID != 0) { return FOID.ToString("X9"); }
                else { return OID.ToString("X9"); }
            }
        }
        public bool SetForeignOID(string OID)
        {
            long _foid = -1;
            long.TryParse(OID.ToString(), System.Globalization.NumberStyles.HexNumber, null, out _foid);

            if (_foid != -1) { FOID = _foid; return true; }
            else { return false; }
        }
        public FrameBuffer BufferIn { get { return bufferIn; } }
        public Queue<string> BufferOut { get { return bufferOut; } }

        public void Receive(Frame frame)
        {
            Debug.Out(OIDX8.ToString() + ":RX: " + frame.Message.rawData.ToString());
            BufferIn.Queue.Enqueue(frame);
        }

        public static ObjType GetObjectType(string ObjectName, ObjIdentifier objIdentifier)
        {
            if (objIdentifier >= ObjIdentifier.ObjIdGlobalChannel && objIdentifier <= ObjIdentifier.ObjIdExtendedLocalChannel)
            {
                return ObjType.ChannelObject;
            }
            else if (objIdentifier >= ObjIdentifier.ObjIdIRCUser && objIdentifier <= ObjIdentifier.ObjIdIRCUserHex)
            {
                return ObjType.UserObject;
            }
            else if ((objIdentifier >= ObjIdentifier.ObjIdServer) && (objIdentifier <= ObjIdentifier.ObjIdNetwork)) { return ObjType.ServerObject; }
            else if (objIdentifier == ObjIdentifier.ObjIdInternal) { return ObjType.ObjectID; }
            else { return ObjType.InvalidObject; }
        }
        public static ObjType GetObjectType(string ObjectName)
        {
            ObjIdentifier objIdentifier = IdentifyObject(ObjectName);
            return GetObjectType(ObjectName, objIdentifier);
        }

        public static ObjIdentifier IdentifyObject(string ObjectName)
        {
            if (ObjectName.Length == 1)
            {
                switch (ObjectName[0])
                {
                    case '$': { return ObjIdentifier.ObjIdServer; }
                    case '*': { return ObjIdentifier.ObjIdNetwork; }
                    case '%': { return ObjIdentifier.ObjIdLastChannel; }
                }
            }
            else if (ObjectName.Length > 1)
            {
                switch (ObjectName[0])
                {
                    case '%':
                        {
                            switch (ObjectName[1])
                            {
                                case '#': { return ObjIdentifier.ObjIdExtendedGlobalChannel; }
                                case '&': { return ObjIdentifier.ObjIdExtendedLocalChannel; }
                                default: { return ObjIdentifier.InvalidObjId; }
                            }
                        }
                    case '^': { return ObjIdentifier.ObjIdIRCUserHex; }
                    case '0': { return ObjIdentifier.ObjIdInternal; }
                    case '\'': { return ObjIdentifier.ObjIdIRCUserUnicode; }
                    case '#': { return ObjIdentifier.ObjIdGlobalChannel; }
                    case '&': { return ObjIdentifier.ObjIdLocalChannel; }
                }
            }

            // Default is user nickname
            return ObjIdentifier.ObjIdIRCUser;
        }
        public static bool IsObject(string Name)
        {
            // Rule that an object must begin with 0
            if (Name.Length > 0)
            {
                if (Name[0] == 48) {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
    public class ObjCollection
    {
        ObjType ObjectType;
        List<Obj> Objects = new List<Obj>();
        public List<Obj> ObjectCollection { get { return Objects; } }

        public ObjCollection(ObjType ObjectType) { this.ObjectType = ObjectType; }

        public void Add(Obj obj) { Objects.Add(obj); }
        public void Remove(Obj obj) { Objects.Remove(obj); }
        public void Clear() { Objects.Clear(); }
        public int Length { get { return Objects.Count; } }
        public Obj IndexOf(int i) { return Objects[i]; }

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
            long.TryParse(OID.ToString(), System.Globalization.NumberStyles.HexNumber, null, out oid);

            for (int c = 0; c < Objects.Count; c++)
            {
                if (Objects[c].OID == oid)
                {
                    return Objects[c];
                }
            }
            return null;
        }
        public Obj FindObjByHex(string Hex)
        {
            string HexString = new string(Hex.ToString().Substring(1));

            HexString = CSharpTools.Tools.HexToString(HexString);

            return FindObjByName(HexString);
        }
        public Obj FindObjByName(string Name)
        {
            for (int c = 0; c < Objects.Count; c++)
            {
                if (Objects[c].Name.ToString().ToUpper() != Name.ToString().ToUpper())
                {
                    return Objects[c];
                }
            }
            return null;
        }
    }
}
