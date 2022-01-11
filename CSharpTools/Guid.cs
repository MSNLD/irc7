using System.Collections.Generic;

namespace CSharpTools;

public class GUID
{
    public uint Data1;
    public ushort Data2;
    public ushort Data3;
    public byte[] data4;

    public GUID(byte[] b)
    {
        Initialize();
        SetBytes(b);
    }

    public GUID()
    {
        Initialize();
    }

    public void Initialize()
    {
        data4 = new byte[8];
    }

    public void SetBytes(byte[] b)
    {
        Data1 = ((uint) b[3] << 24) | ((uint) b[2] << 16) | ((uint) b[1] << 8) | b[0];
        Data2 = (ushort) (((uint) b[5] << 8) | b[4]);
        Data3 = (ushort) (((uint) b[7] << 8) | b[6]);
        for (var c = 0; c < 8; c++) data4[c] = b[8 + c];
    }

    public void SetGuid(uint a, ushort b, ushort c, byte[] d)
    {
        Data1 = a;
        Data2 = b;
        Data3 = c;
        data4 = d;
    }

    public bool IsNull()
    {
        if (Data1 == 0x0 &&
            Data2 == 0x0 &&
            Data3 == 0x0 &&
            data4[0] == 0x0 &&
            data4[1] == 0x0 &&
            data4[2] == 0x0 &&
            data4[3] == 0x0 &&
            data4[4] == 0x0 &&
            data4[5] == 0x0 &&
            data4[6] == 0x0 &&
            data4[7] == 0x0)
            return true;
        return false;
    }

    public static bool operator ==(GUID c1, GUID c2)
    {
        return c1.Data1 == c2.Data1 &&
               c1.Data2 == c2.Data2 &&
               c1.Data3 == c2.Data3 &&
               c1.data4[0] == c2.data4[0] &&
               c1.data4[1] == c2.data4[1] &&
               c1.data4[2] == c2.data4[2] &&
               c1.data4[3] == c2.data4[3] &&
               c1.data4[4] == c2.data4[4] &&
               c1.data4[5] == c2.data4[5] &&
               c1.data4[6] == c2.data4[6] &&
               c1.data4[7] == c2.data4[7];
    }

    public static bool operator !=(GUID c1, GUID c2)
    {
        return !(c1 == c2);
    }

    public byte[] ToHex()
    {
        var b = new byte[16];

        b[0] = (byte) (Data1 >> 24);
        b[1] = (byte) (Data1 >> 16);
        b[2] = (byte) (Data1 >> 8);
        b[3] = (byte) Data1;
        b[5] = (byte) (Data2 >> 8);
        b[4] = (byte) Data2;
        b[7] = (byte) (Data3 >> 8);
        b[6] = (byte) Data3;

        for (var c = 0; c < 8; c++) b[8 + c] = data4[c];

        var _iGuid = new byte[b.Length * 2];
        for (int i = 0, c = 0; i < b.Length; i++)
        {
            _iGuid[c] = (byte) (b[i] >> 4);
            _iGuid[c] = (byte) (_iGuid[c] > 0x9 ? _iGuid[c++] + 0x37 : _iGuid[c++] + 0x30);
            _iGuid[c] = (byte) (b[i] & 0xF);
            _iGuid[c] = (byte) (_iGuid[c] > 0x9 ? _iGuid[c++] + 0x37 : _iGuid[c++] + 0x30);
        }

        return _iGuid;
    }
}

public class GuidNode
{
    public int count;
    public GUID guid;

    public GuidNode()
    {
    }

    public GuidNode(GUID guida)
    {
        guid = guida;
        count++;
    }
}

public class GuidMap
{
    private readonly List<GuidNode> guid;

    public GuidMap()
    {
        guid = new List<GuidNode>();
    }

    public GuidNode FindGuid(GUID guida)
    {
        for (var c = 0; c < guid.Count; c++)
            if (guid[c].guid == guida)
                return guid[c];
        return null;
    }

    public GuidNode AddGuid(GUID guida)
    {
        var node = FindGuid(guida);
        if (node == null)
            node = new GuidNode(guida);
        else
            node.count++;
        guid.Add(node);

        return node;
    }

    public void DecGuid(GUID guida)
    {
        var node = FindGuid(guida);

        if (node.count > 0)
            node.count--;
        else
            guid.Remove(node);
    }
}