using System.Runtime.InteropServices;
using Irc.ClassExtensions.CSharpTools;
using Irc.Extensions.Security.Packages;

namespace Irc.Extensions.Apollo.Security.Packages;

public class GateKeeperTokenHelper
{
    public static GateKeeperToken InitializeFromBytes(byte[] Data)
    {
        var AuthToken = new GateKeeperToken();
        var pBuf = Marshal.AllocHGlobal(Marshal.SizeOf(AuthToken));
        try
        {
            Marshal.Copy(Data, 0, pBuf, Marshal.SizeOf(AuthToken));
            AuthToken = Marshal.PtrToStructure<GateKeeperToken>(pBuf);
        }
        catch (Exception)
        {
        }
        finally
        {
            Marshal.FreeHGlobal(pBuf);
        }

        return AuthToken;
    }

    public static GateKeeperToken CreateGateKeeperToken(string signature)
    {
        var AuthToken = new GateKeeperToken();
        AuthToken.Signature = signature.ToByteArray();
        return AuthToken;
    }

    public static GateKeeperToken CreateGuid()
    {
        var AuthToken = new GateKeeperToken();
        InitializeFromBytes(new Guid().ToByteArray());
        return AuthToken;
    }

    public static byte[] GetBytes(GateKeeperToken AuthToken)
    {
        var ptrSize = Marshal.SizeOf(AuthToken);
        var _iGuid = new byte[ptrSize];
        var pBuf = Marshal.AllocHGlobal(ptrSize);
        try
        {
            Marshal.StructureToPtr(AuthToken, pBuf, false);

            for (var i = 0; i < ptrSize; i++) _iGuid[i] = Marshal.ReadByte(pBuf, i);
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            Marshal.FreeHGlobal(pBuf);
        }

        return _iGuid;
    }
}