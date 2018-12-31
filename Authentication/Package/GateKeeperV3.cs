using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using CSharpTools;
using System.Runtime.InteropServices;


namespace Core.Authentication.Package
{
    class GateKeeper : SSP { 
        public static new String8 DOMAIN = new String8("GateKeeper");
        public static String8 _gksspSig = new String8("GKSSP\0");
        public const UInt64 SIGNATURE = 0x0000005053534b47; //S2 0x0000005053534b47 ulong
        public new string NicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";
        public override UInt64 Signature { get { return SIGNATURE; } }

        protected GKSSPToken ServerToken, ClientToken;
        protected GUID ClientGUID;
        private String8 challenge;

        // Credit to JD for discovering the below key through XOR'ing (Discovered 2017/05/04)
        static String8 key = "SRFMKSJANDRESKKC";
        public GateKeeper()
        {
            guest = true;
            server_sequence = (int)state.SSP_INIT;
        }

        public override state InitializeSecurityContext(String8 data, String8 ip)
        {
            String8 lit = String8.ToLiteral(data);
            if (lit.length >= 0x10)
            {
                if (String8.compare(lit, _gksspSig, 0x10))
                {
                    ClientToken = GKSSPTokenHelper.InitializeFromBytes(lit.bytes);
                    if ((ClientToken.Sequence == server_sequence) && ((ClientToken.Version >= 2) && (ClientToken.Version <= 3)))
                    {
                        server_sequence = (int)state.SSP_EXT; //expecting a SSP_EXT reply after challenge is sent
                        server_version = ClientToken.Version;
                        return state.SSP_OK;
                    }
                }
            }
            return state.SSP_FAILED;
        }
        public override state AcceptSecurityContext(String8 data, String8 ip)
        {
            String8 lit = String8.ToLiteral(data);
            if (lit.length >= 0x20)
            {
                if (String8.compare(lit, _gksspSig, 0x10))
                {
                    ClientToken = GKSSPTokenHelper.InitializeFromBytes(lit.bytes);
                    uint _clientVersion = ClientToken.Version, _clientStage = (uint)ClientToken.Sequence;
                    if ((_clientStage == server_sequence) && ((_clientVersion >= 2) && (_clientVersion <= 3)))
                    {
                        String8 context = new String8(lit.bytes, 16, 32);
                        if (VerifySecurityContext(challenge, context.bytes, ip, server_version))
                        {
                            //Note that I need to improve the below code. Guid needs a ToByteArray() function
                            String8 GuidBinary = null;

                            if (lit.length >= 0x30) {
                                GuidBinary = new String8(lit.bytes, 32, 48);
                                ClientGUID = new GUID(GuidBinary.bytes);
                            }
                            else {
                                GuidBinary = new String8(Guid.NewGuid().ToByteArray(), 0, 16);
                                ClientGUID = new GUID(GuidBinary.bytes);
                            }

                            if ((!ClientGUID.IsNull()) || (guest == false))
                            {
                                uuid = ClientGUID.ToHex();
                                memberIdLow = BitConverter.ToUInt64(GuidBinary.bytes, 0);
                                memberIdHigh = BitConverter.ToUInt64(GuidBinary.bytes, 8);
                                server_sequence = (int)state.SSP_AUTHENTICATED;
                                IsAuthenticated = true;
                                return state.SSP_OK;
                            }
                        }
                    }
                }
            }
            return state.SSP_FAILED;
        }
        public override SSP Create() { return new GateKeeper(); }
        public override String8 GetDomain() { return GateKeeper.DOMAIN; }
        public override string GetNickMask() { return NicknameMask; }

        public bool VerifySecurityContext(String8 challenge, byte[] context, String8 ip, uint version)
        {
            HMACMD5 md5 = new HMACMD5(key.bytes);
            String8 ctx = new String8(challenge.length + ip.length);
            ctx.append(challenge);
            if (version == 3) { ctx.append(ip); }
            byte[] h1 = md5.ComputeHash(ctx.bytes, 0, ctx.length);
            return (String8.memcmp(context, h1, 16) == 0);
        }
        public override String8 CreateSecurityChallenge(state stage)
        {
            if (stage == state.SSP_SEC)
            {
                ServerToken = GKSSPTokenHelper.CreateGateKeeperToken();
                challenge = new String8(Guid.NewGuid().ToByteArray(), 0, 8);
                for (int i = 0; i < challenge.length; i++) { challenge.bytes[i] = (byte)(challenge.bytes[i] % 0x7F); } // for mIRC
                String8 message = new String8(Marshal.SizeOf<GKSSPToken>(ServerToken) + challenge.bytes.Length); //create new message with full size
                ServerToken.Version = ClientToken.Version;
                ServerToken.Sequence = 2;
                message.append(GKSSPTokenHelper.GetBytes(ServerToken));
                message.append(challenge.bytes);
                return message;
            }
            return null;
        }
    }

    public struct GKSSPToken
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Signature;
        public UInt16 TimeStamp;
        public uint Version;
        public uint Sequence;
    }
    public class GKSSPTokenHelper
    {
        public static GKSSPToken InitializeFromBytes(byte[] Data)
        {
            GKSSPToken AuthToken = new GKSSPToken();
            IntPtr pBuf = Marshal.AllocHGlobal(Marshal.SizeOf(AuthToken));
            try
            {
                Marshal.Copy(Data, 0, pBuf, Marshal.SizeOf(AuthToken));
                AuthToken = Marshal.PtrToStructure<GKSSPToken>(pBuf);
            }
            catch (Exception e) { }
            finally { Marshal.FreeHGlobal(pBuf); }
            return AuthToken;
        }
        public static GKSSPToken CreateGateKeeperToken()
        {
            GKSSPToken AuthToken = new GKSSPToken();
            AuthToken.Signature = GateKeeper._gksspSig.bytes;
            return AuthToken;
        }
        public static GKSSPToken CreateGuid()
        {
            GKSSPToken AuthToken = new GKSSPToken();
            InitializeFromBytes((new Guid()).ToByteArray());
            return AuthToken;
        }
        public static byte[] GetBytes(GKSSPToken AuthToken)
        {
            int ptrSize = Marshal.SizeOf(AuthToken);
            byte[] _iGuid = new byte[ptrSize];
            IntPtr pBuf = Marshal.AllocHGlobal(ptrSize);
            try
            {
                Marshal.StructureToPtr(AuthToken, pBuf, false);

                for (int i = 0; i < ptrSize; i++)
                {
                    _iGuid[i] = Marshal.ReadByte(pBuf, i);
                }
            }
            catch (Exception e) { return null; }
            finally { Marshal.FreeHGlobal(pBuf); }
            return _iGuid;
        }
    }

}
