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
        Queue<String8> bufferOut = new Queue<String8>();
        String8 _intName = null, _intUName = null;
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

        public String8 Name {
            get {
                return (_intName == null ? Resources.Wildcard : _intName);
            }
            set {
                Properties.Name.Value = value;
                _intName = value;
                _intUName = new String8(_intName.bytes);
                _intUName.toupper();
            } }
        public String8 UCaseName { get { return _intUName; } }
        public String8 OIDX8 {
            get {
                if (FOID != 0) { return FOID.ToString("X9"); }
                else { return OID.ToString("X9"); }
            }
        }
        public bool SetForeignOID(String8 OID)
        {
            long _foid = -1;
            long.TryParse(OID.chars, System.Globalization.NumberStyles.HexNumber, null, out _foid);

            if (_foid != -1) { FOID = _foid; return true; }
            else { return false; }
        }
        public FrameBuffer BufferIn { get { return bufferIn; } }
        public Queue<String8> BufferOut { get { return bufferOut; } }

        public void Receive(Frame frame)
        {
            Debug.Out(OIDX8.chars + ":RX: " + frame.Message.rawData.chars);
            BufferIn.Queue.Enqueue(frame);
        }

        public static ObjType GetObjectType(String8 ObjectName, ObjIdentifier objIdentifier)
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
        public static ObjType GetObjectType(String8 ObjectName)
        {
            ObjIdentifier objIdentifier = IdentifyObject(ObjectName);
            return GetObjectType(ObjectName, objIdentifier);
        }

        public static ObjIdentifier IdentifyObject(String8 ObjectName)
        {
            if (ObjectName.Length == 1)
            {
                switch (ObjectName.bytes[0])
                {
                    case (byte)'$': { return ObjIdentifier.ObjIdServer; }
                    case (byte)'*': { return ObjIdentifier.ObjIdNetwork; }
                    case (byte)'%': { return ObjIdentifier.ObjIdLastChannel; }
                }
            }
            else if (ObjectName.Length > 1)
            {
                switch (ObjectName.bytes[0])
                {
                    case (byte)'%':
                        {
                            switch (ObjectName.bytes[1])
                            {
                                case (byte)'#': { return ObjIdentifier.ObjIdExtendedGlobalChannel; }
                                case (byte)'&': { return ObjIdentifier.ObjIdExtendedLocalChannel; }
                                default: { return ObjIdentifier.InvalidObjId; }
                            }
                        }
                    case (byte)'^': { return ObjIdentifier.ObjIdIRCUserHex; }
                    case (byte)'0': { return ObjIdentifier.ObjIdInternal; }
                    case (byte)'\'': { return ObjIdentifier.ObjIdIRCUserUnicode; }
                    case (byte)'#': { return ObjIdentifier.ObjIdGlobalChannel; }
                    case (byte)'&': { return ObjIdentifier.ObjIdLocalChannel; }
                }
            }

            // Default is user nickname
            return ObjIdentifier.ObjIdIRCUser;
        }
        public static bool IsObject(String8 Name)
        {
            // Rule that an object must begin with 0
            if (Name.Length > 0)
            {
                if (Name.bytes[0] == 48) {
                    return true;
                }
            }

            return false;
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

        public Obj FindObj(String8 Name, ObjIdentifier ObjType)
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

        public Obj FindObjByOID(String8 OID)
        {
            long oid;
            long.TryParse(OID.chars, System.Globalization.NumberStyles.HexNumber, null, out oid);

            for (int c = 0; c < Objects.Count; c++)
            {
                if (Objects[c].OID == oid)
                {
                    return Objects[c];
                }
            }
            return null;
        }
        public Obj FindObjByHex(String8 Hex)
        {
            String8 HexString = new String8(Hex.bytes, 1, Hex.length);

            HexString = CSharpTools.Tools.HexToString(HexString);

            return FindObjByName(HexString);
        }
        public Obj FindObjByName(String8 Name)
        {
            for (int c = 0; c < Objects.Count; c++)
            {
                if (!String8.compareCaseInsensitive(Objects[c].Name, Name))
                {
                    return Objects[c];
                }
            }
            return null;
        }
    }
}
