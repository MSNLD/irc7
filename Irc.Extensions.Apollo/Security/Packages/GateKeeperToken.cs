using System.Runtime.InteropServices;

namespace Irc.Extensions.Apollo.Security.Packages;

public struct GateKeeperToken
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] Signature;

    public ushort TimeStamp;
    public uint Version;
    public uint Sequence;
}