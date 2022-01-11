using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using CSharpTools;
using System.Runtime.InteropServices;
using System.Linq;
using Core.CSharpTools;

namespace Core.Authentication.Package
{
    class GateKeeper : SSP
    {
        public static new string DOMAIN = "GateKeeper";
        public static string _gksspSig = "GKSSP\0";
        public const UInt64 SIGNATURE = 0x0000005053534b47; //S2 0x0000005053534b47 ulong
        public new string NicknameMask = @"^>(?!(Sysop)|(Admin)|(Guide))[\x41-\xFF\-0-9]+$";
        public override UInt64 Signature { get { return SIGNATURE; } }

        protected GKSSPToken ServerToken, ClientToken;
        protected GUID ClientGUID;
        private string challenge;

        // Credit to JD for discovering the below key through XOR'ing (Discovered 2017/05/04)
        static string key = "SRFMKSJANDRESKKC";
        public GateKeeper()
        {
            guest = true;
            server_sequence = (int)state.SSP_INIT;
        }

        public override state InitializeSecurityContext(string data, string ip)
        {
            StringBuilder lit = StringBuilderExtensions.ToLiteral(data);
            if (lit.Length >= 0x10)
            {
                if (lit.ToString().StartsWith(_gksspSig))
                {
                    ClientToken = GKSSPTokenHelper.InitializeFromBytes(lit.ToByteArray());
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
        public override state AcceptSecurityContext(string data, string ip)
        {
            StringBuilder lit = StringBuilderExtensions.ToLiteral(data);
            if (lit.Length >= 0x20)
            {
                if (lit.ToString().StartsWith(_gksspSig))
                {
                    ClientToken = GKSSPTokenHelper.InitializeFromBytes(lit.ToByteArray());
                    uint _clientVersion = ClientToken.Version, _clientStage = (uint)ClientToken.Sequence;
                    if ((_clientStage == server_sequence) && ((_clientVersion >= 2) && (_clientVersion <= 3)))
                    {
                        string context = StringBuilderExtensions.FromBytes(lit.ToByteArray(), 16, 32).ToString();
                        if (VerifySecurityContext(challenge, context.ToByteArray(), ip, server_version))
                        {
                            //Note that I need to improve the below code. Guid needs a ToByteArray() function
                            StringBuilder GuidBinary = null;

                            if (lit.Length >= 0x30)
                            {
                                GuidBinary = StringBuilderExtensions.FromBytes(lit.ToByteArray(), 32, 48);
                                ClientGUID = new GUID(GuidBinary.ToByteArray());
                            }
                            else
                            {
                                GuidBinary = StringBuilderExtensions.FromBytes(Guid.NewGuid().ToByteArray(), 0, 16);
                                ClientGUID = new GUID(GuidBinary.ToByteArray());
                            }

                            if ((!ClientGUID.IsNull()) || (guest == false))
                            {
                                uuid = ClientGUID.ToHex();
                                memberIdLow = BitConverter.ToUInt64(GuidBinary.ToByteArray(), 0);
                                memberIdHigh = BitConverter.ToUInt64(GuidBinary.ToByteArray(), 8);
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
        public override string GetDomain() { return GateKeeper.DOMAIN; }
        public override string GetNickMask() { return NicknameMask; }

        public bool VerifySecurityContext(string challenge, byte[] context, string ip, uint version)
        {
            HMACMD5 md5 = new HMACMD5(key.ToByteArray());
            StringBuilder ctx = new StringBuilder(challenge.Length + ip.Length);
            ctx.Append(challenge);
            if (version == 3) { ctx.Append(ip); }
            byte[] h1 = md5.ComputeHash(ctx.ToByteArray(), 0, ctx.Length);
            return h1.SequenceEqual(context);
        }
        public override string CreateSecurityChallenge(state stage)
        {
            if (stage == state.SSP_SEC)
            {
                ServerToken = GKSSPTokenHelper.CreateGateKeeperToken();
                //challenge = StringBuilderExtensions.FromBytes(Guid.NewGuid().ToByteArray(), 0, 8).ToString();
                challenge = "AAAAAAAA";
                for (int i = 0; i < challenge.Length; i++) { challenge.ToByteArray()[i] = (byte)(challenge.ToByteArray()[i] % 0x7F); } // for mIRC
                StringBuilder message = new StringBuilder(Marshal.SizeOf<GKSSPToken>(ServerToken) + challenge.ToByteArray().Length); //create new message with full size
                ServerToken.Version = ClientToken.Version;
                ServerToken.Sequence = 2;
                message.AppendByteArrayAsChars(GKSSPTokenHelper.GetBytes(ServerToken));
                message.AppendByteArrayAsChars(challenge.ToByteArray());
                return message.ToString();
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
            AuthToken.Signature = GateKeeper._gksspSig.ToByteArray();
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
