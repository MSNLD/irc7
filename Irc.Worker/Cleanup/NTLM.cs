using System;
using System.Collections.Generic;
using System.Text;
using CSharpTools;
using System.Runtime.InteropServices;
using Core.CSharpTools;

namespace Core.Authentication.Package
{
    // NTLM Implementation by Sky
    // Created: Long time ago...
    // NTLM is required for the CAC to work

    class NTLM : SSP
    {
        public static new String8 DOMAIN = new String8(Program.Config.NTLMDomain);
        public static String8 NTLMMessage = new String8("NTLMSSP\0\0\0\0\0\0\0\0\0");
        public static String8 NTLMSSPSig = new String8("NTLMSSP\0");
        public const UInt64 SIGNATURE = 0x005053534d4c544e; //S1 0x005053534d4c544e ulong

        Guid seed, context, puid, message;
        bool passport;

        public override string GetNickMask() { return NicknameMask; }

        #region NTLM Sec Buff
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPSecurityBuffer
        {
            [MarshalAs(UnmanagedType.I2)]
            public short Length;
            [MarshalAs(UnmanagedType.I2)]
            public short AllocatedSpace;
            [MarshalAs(UnmanagedType.I4)]
            public int Offset;
        }
        #endregion

        #region NTLM Sub Block
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPSubBlock
        {
            //public string Data;
            [MarshalAs(UnmanagedType.I2)]
            public short Type;
            [MarshalAs(UnmanagedType.I2)]
            public short Length;
        }
        #endregion

        #region NTLM Message Type 1
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPMessageType1
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Signature;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Type;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Flags;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer SuppliedDomain;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer SuppliedWorkstation;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer OSVersionInfo;
        }
        #endregion

        #region NTLM Message Type 2
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPMessageType2
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Signature;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Type;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetName;

            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Challenge;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Context;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetInformation;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
            public NTLMSSPSubBlock DomainName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
            public NTLMSSPSubBlock ServerName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
            public NTLMSSPSubBlock DNSDomainName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 4)]
            public NTLMSSPSubBlock DNSServerName;

            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public short TT;
            [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
            public short TL;
        }

        //            string CredentialsHandle = Marshal.PtrToStringAnsi(ipMessage2, Marshal.SizeOf(Message2));
        //CredentialsHandle = CredentialsHandle.Insert(24, Challenge);
        //CredentialsHandle = CredentialsHandle.Insert(32, Context);
        //CredentialsHandle = CredentialsHandle.Insert(Message2.TargetName.Offset, TargetNameData);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPMessageType2Client
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Signature;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Type;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetName;

            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Challenge;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string Context;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetInformation;
        }
        #endregion

        #region NTLM Message Type 3
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPMessageType3
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Signature;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Type;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer LMResponse;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer NTLMResponse;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer UserName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer WorkstationName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer SessionKey;

            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Flags;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Challenge;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Context;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct NTLMSSPMessageType3Client
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Signature;
            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Type;

            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer LMResponse;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer NTLMResponse;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer TargetName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer UserName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer WorkstationName;
            //[MarshalAs(UnmanagedType.Struct, SizeConst = 8)]
            public NTLMSSPSecurityBuffer SessionKey;

            [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
            public uint Flags;
        }
        #endregion

        #region NTLM Constants
        String8 NTLM_SIGNATURE = new String8("NTLMSSP\0");
        String8 NTLM_DES_CONST = new String8("KGS!@#$%");
        uint NTLMTypeNegotiate = 0x01;
        uint NTLMTypeChallenge = 0x02;
        uint NTLMTypeAuthentication = 0x03;

        uint NTLM_NEGOTIATE_UNICODE = 0x00000001;
        uint NTLM_NEGOTIATE_OEM = 0x00000002;
        //uint NTLM_REQUEST_TARGET = 0x00000004;
        uint NTLM_NEGOTIATE_NTLM = 0x00000200;
        uint NTLM_NEGOTIATE_DOMAIN_SUPPLIED = 0x00001000;
        uint NTLM_NEGOTIATE_WORKSTATION_SUPPLIED = 0x00002000;
        //uint NTLM_ALWAYS_SIGN = 0x00008000;
        //uint NTLM_TARGET_TYPE_DOMAIN = 0x00010000;
        //uint NTLM_TARGET_TYPE_DOMAIN = 0x00010000;
        //uint NTLM_NTLM2_KEY = 0x00080000; //not used?
        //uint NTLM_NEGOTIATE_TARGET_INFO = 0x00800000;
        //uint NTLM_NEGOTIATE_128 = 0x20000000;
        //uint NTLM_NEGOTIATE_56 = 0x80000000;

        ushort NTLM_SERVER_NAME_SUBBLOCK = 0x1;
        ushort NTLM_DOMAIN_NAME_SUBBLOCK = 0x2;
        ushort DNS_SERVER_NAME_SUBBLOCK = 0x3;
        ushort DNS_DOMAIN_NAME_SUBBLOCK = 0x4;

        ushort NTLM_MESSAGE_TYPE_1_REQUIRED_LENGTH = 40;
        ushort NTLM_MESSAGE_TYPE_2_REQUIRED_LENGTH = 48;
        //ushort NTLM_MESSAGE_TYPE_3_REQUIRED_LENGTH = 64;

        public enum NTLMOption { LMAuthOnly = -1, NTLM = 0, NTLMAuthOnly = 1 };

        #endregion

        string LastError;

        NTLMSSPMessageType1 Message1;
        NTLMSSPMessageType2 Message2;
        NTLMSSPMessageType3 Message3;
        NTLMSSPMessageType2Client Message2Client;
        NTLMSSPMessageType3Client Message3Client;
        public NTLMOption NTLMOptions;
        uint ClientFlags;
        public String8 ClientUsername;
        public String8 ClientPassword;
        public String8 ClientDomain;
        public String8 ClientWorkstation;

        String8 Challenge;
        String8 ServerDomain;
        String8 ServerName;
        String8 DNSDomain;
        String8 DNSServer;

        String8 LMResponse;
        String8 NTLMResponse;
        String8 LMChallenge;
        String8 NTLMChallenge;

        DES Des;
        String8 CurrentKey;

        public NTLM()
        {
            NTLMOptions = NTLMOption.NTLMAuthOnly;
            ServerDomain = new String8(Program.Config.NTLMDomain);
            ServerName = new String8(Program.Config.ServerName);
            DNSDomain = new String8(Program.Config.NTLMFQDN);
            DNSServer = new String8(Program.Config.NTLMServerDomain);
            Des = new DES();
        }

        public override SSP Create() { return new NTLM(); }
        public override String8 GetDomain() { return NTLM.DOMAIN; }
        public override UInt64 Signature { get { return SIGNATURE; } }

        public bool IsNTLMSupported()
        {
            return Convert.ToBoolean(ClientFlags & NTLM_NEGOTIATE_NTLM);
        }
        public bool IsDomainSupplied()
        {
            return Convert.ToBoolean(ClientFlags & NTLM_NEGOTIATE_DOMAIN_SUPPLIED);
        }
        public bool IsWorkstationSupplied()
        {
            return Convert.ToBoolean(ClientFlags & NTLM_NEGOTIATE_WORKSTATION_SUPPLIED);
        }

        public String8 GetType1Message()
        {
            String8 UnicodeDomain = new String8(String8.toutf16(ClientDomain.bytes));
            String8 UnicodeWorkstation = new String8(String8.toutf16(ClientWorkstation.bytes));

            Message1.Signature = NTLMSSPSig.bytes;
            Message1.Flags = (NTLM_NEGOTIATE_NTLM | NTLM_NEGOTIATE_DOMAIN_SUPPLIED | NTLM_NEGOTIATE_WORKSTATION_SUPPLIED);
            Message1.OSVersionInfo.AllocatedSpace = 0;
            Message1.OSVersionInfo.Length = 0;
            Message1.OSVersionInfo.Offset = 0;

            Message1.SuppliedDomain.AllocatedSpace = (short)UnicodeDomain.length;
            Message1.SuppliedDomain.Length = (short)UnicodeDomain.length;
            Message1.SuppliedDomain.Offset = 40;

            Message1.SuppliedWorkstation.AllocatedSpace = (short)UnicodeWorkstation.length;
            Message1.SuppliedWorkstation.Length = (short)UnicodeWorkstation.length;
            Message1.SuppliedWorkstation.Offset = 40 + UnicodeDomain.length;

            Message1.Type = 1;


            // Recoded to safely handle Marshal pointers
            int ptrMessageSize = Marshal.SizeOf(Message1);
            String8 bytMessage = new String8(ptrMessageSize + UnicodeDomain.length + UnicodeWorkstation.length);
            IntPtr ipMessage1 = Marshal.AllocHGlobal(ptrMessageSize);
            try
            {
                Marshal.StructureToPtr(Message1, ipMessage1, false);
                for (int i = 0; i < ptrMessageSize; i++)
                {
                    bytMessage.bytes[i] = (char)Marshal.ReadByte(ipMessage1, i);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ipMessage1);
            }
            bytMessage.length = 40;
            bytMessage.append(UnicodeDomain.bytes.GetBytes());
            bytMessage.append(UnicodeWorkstation.bytes.GetBytes());

            return bytMessage;
        }

        public override state InitializeSecurityContext(String8 data, String8 ip)
        {
            int NTLMSSPMessageType1Size = Marshal.SizeOf<NTLMSSPMessageType1>();
            data = StringExtensions.ToLiteral(data);

            if (data.length < NTLMSSPMessageType1Size) { return state.SSP_FAILED; }


            IntPtr pBuf = IntPtr.Zero;
            byte[] NTLM1Header = new byte[NTLMSSPMessageType1Size];
            Buffer.BlockCopy(data.bytes, 0, NTLM1Header, 0, NTLMSSPMessageType1Size);

            try
            {
                pBuf = Marshal.AllocHGlobal(NTLMSSPMessageType1Size);
                Marshal.Copy(NTLM1Header, 0, pBuf, NTLMSSPMessageType1Size);
                Message1 = (NTLMSSPMessageType1)Marshal.PtrToStructure<NTLMSSPMessageType1>(pBuf);
            }
            catch (Exception e)
            {
                return state.SSP_FAILED;
            }
            finally
            {
                if (pBuf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pBuf);
                }
            }

            String8 clientSig = new String8(Message1.Signature);
            ClientFlags = Message1.Flags;
            if ((NTLMSSPSig == clientSig) && (Message1.Type == NTLMTypeNegotiate) && (IsNTLMSupported()))
            {
                if (data.length >= NTLM_MESSAGE_TYPE_1_REQUIRED_LENGTH)
                {
                    if ((IsDomainSupplied()) && (Message1.SuppliedDomain.Offset + Message1.SuppliedDomain.Length <= data.length))
                    {
                        ClientDomain = new String8(data.bytes, Message1.SuppliedDomain.Offset, Message1.SuppliedDomain.Offset + Message1.SuppliedDomain.Length);
                    }
                    else { ClientDomain = ServerDomain; }

                    if ((IsWorkstationSupplied()) && (Message1.SuppliedWorkstation.Offset + Message1.SuppliedWorkstation.Length <= data.length))
                    {
                        ClientWorkstation = new String8(data.bytes, Message1.SuppliedWorkstation.Offset, Message1.SuppliedWorkstation.Offset + Message1.SuppliedWorkstation.Length);
                    }
                    else { ClientWorkstation = Resources.Null; }

                    return state.SSP_OK;
                }
                else { return state.SSP_FAILED; }
            }

            seed = Guid.NewGuid();
            return state.SSP_OK;
        }
        public override String8 CreateSecurityChallenge(state stage)
        {

            Message2.Signature = NTLM_SIGNATURE.bytes;
            Message2.Flags = NTLM_NEGOTIATE_UNICODE +
                         NTLM_NEGOTIATE_OEM +
                         NTLM_NEGOTIATE_DOMAIN_SUPPLIED +
                         NTLM_NEGOTIATE_WORKSTATION_SUPPLIED +
                         NTLM_NEGOTIATE_NTLM;
            Message2.Type = NTLMTypeChallenge;

            String8 TargetNameData = new String8(String8.toutf16(ClientDomain.bytes));

            String8 M2DomainName = new String8(String8.toutf16(ServerDomain.bytes));
            Message2.DomainName.Type = (short)NTLM_DOMAIN_NAME_SUBBLOCK;
            Message2.DomainName.Length = (short)M2DomainName.length;

            String8 M2ServerName = new String8(String8.toutf16(ServerName.bytes));
            Message2.ServerName.Type = (short)NTLM_SERVER_NAME_SUBBLOCK;
            Message2.ServerName.Length = (short)M2ServerName.length;

            String8 M2DNSDomainName = new String8(String8.toutf16(DNSDomain.bytes));
            Message2.DNSDomainName.Type = (short)DNS_DOMAIN_NAME_SUBBLOCK;
            Message2.DNSDomainName.Length = (short)M2DNSDomainName.length;

            String8 M2DNSServerName = new String8(String8.toutf16(DNSServer.bytes));
            Message2.DNSServerName.Type = (short)DNS_SERVER_NAME_SUBBLOCK;
            Message2.DNSServerName.Length = (short)M2DNSServerName.length;

            Challenge = (new String8(Guid.NewGuid().ToByteArray(), 8, 16));
            Message2.Challenge = Challenge.bytes;

            Message2.Context = (new String8("\0\0\0\0\0\0\0\0")).bytes;

            Message2.TargetName.Offset = NTLM_MESSAGE_TYPE_2_REQUIRED_LENGTH;
            Message2.TargetName.Length = (short)TargetNameData.length;
            Message2.TargetName.AllocatedSpace = (short)TargetNameData.length;

            Message2.TargetInformation.Offset = NTLM_MESSAGE_TYPE_2_REQUIRED_LENGTH + Message2.TargetName.Length;
            Message2.TargetInformation.Length = (short)(20 + Message2.DomainName.Length + Message2.ServerName.Length + Message2.DNSDomainName.Length + Message2.DNSServerName.Length); // 20 = 4*4 = 16 + 4 = 20
            Message2.TargetInformation.AllocatedSpace = (short)Message2.TargetInformation.Length;

            int ptrMessageSize = Marshal.SizeOf<NTLMSSPMessageType2>();
            String8 bytMessage = new String8(ptrMessageSize);
            IntPtr ipMessage2 = Marshal.AllocHGlobal(ptrMessageSize);
            try
            {
                Marshal.StructureToPtr(Message2, ipMessage2, false);

                for (int i = 0; i < ptrMessageSize; i++)
                {
                    bytMessage.bytes[i] = Marshal.ReadByte(ipMessage2, i);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(ipMessage2);
            }

            String8 CredentialsHandle = new String8(ptrMessageSize + TargetNameData.bytes.Length + M2DomainName.bytes.Length + M2ServerName.bytes.Length + M2DNSDomainName.bytes.Length + M2DNSServerName.bytes.Length);
            CredentialsHandle.append(bytMessage.bytes, 0, 48);
            CredentialsHandle.append(TargetNameData.bytes);

            int Offset = 48;
            CredentialsHandle.append(bytMessage.bytes, Offset, Offset + 4); Offset += 4;
            CredentialsHandle.append(M2DomainName.bytes);
            CredentialsHandle.append(bytMessage.bytes, Offset, Offset + 4); Offset += 4;
            CredentialsHandle.append(M2ServerName.bytes);
            CredentialsHandle.append(bytMessage.bytes, Offset, Offset + 4); Offset += 4;
            CredentialsHandle.append(M2DNSDomainName.bytes);
            CredentialsHandle.append(bytMessage.bytes, Offset, Offset + 4); Offset += 4;
            CredentialsHandle.append(M2DNSServerName.bytes);
            CredentialsHandle.append(bytMessage.bytes, Offset, Offset + 4); Offset += 4;

            return CredentialsHandle;

        }

        public state AcceptType2Message(String8 data)
        {
            data = StringExtensions.ToLiteral(data);
            IntPtr pBuf = Marshal.AllocHGlobal(Marshal.SizeOf(Message2));
            try
            {
                Marshal.Copy(data.bytes, 0, pBuf, Marshal.SizeOf(Message2));
                Message2 = (NTLMSSPMessageType2)Marshal.PtrToStructure<NTLMSSPMessageType2>(pBuf);
                Challenge = new String8(Message2.Challenge);
            }
            catch (Exception e)
            {
                return state.SSP_FAILED;
            }
            finally
            {
                Marshal.FreeHGlobal(pBuf);
            }
            return state.SSP_OK;
        }
        public String8 GetSecurityContext(String8 username, String8 password)
        {
            VerifySecurityContext(Challenge, password);
            //NTLMChallenge, LMChallenge will have the challenge hash stored

            String8 UnicodeUsername = new String8(String8.toutf16(username.bytes));
            String8 UnicodeDomain = new String8(String8.toutf16(ClientDomain.bytes));
            String8 UnicodeWorkstation = new String8(String8.toutf16(ClientWorkstation.bytes));


            int M3Len = Marshal.SizeOf(Message3Client);

            Message3Client.Signature = NTLMSSPSig.bytes;
            Message3Client.Type = 3;

            Message3Client.LMResponse.AllocatedSpace = 24;
            Message3Client.LMResponse.Length = 24;
            Message3Client.LMResponse.Offset = M3Len;

            Message3Client.NTLMResponse.AllocatedSpace = 24;
            Message3Client.NTLMResponse.Length = 24;
            Message3Client.NTLMResponse.Offset = M3Len + 24;

            Message3Client.TargetName.AllocatedSpace = (short)UnicodeDomain.length;
            Message3Client.TargetName.Length = (short)UnicodeDomain.length;
            Message3Client.TargetName.Offset = M3Len + 24 + 24;

            Message3Client.UserName.AllocatedSpace = (short)UnicodeUsername.length;
            Message3Client.UserName.Length = (short)UnicodeUsername.length;
            Message3Client.UserName.Offset = M3Len + 24 + 24 + UnicodeDomain.length;

            //skip workstation rubbish...

            //skip session key

            Message3Client.Flags = (NTLM_NEGOTIATE_NTLM | NTLM_NEGOTIATE_DOMAIN_SUPPLIED | NTLM_NEGOTIATE_WORKSTATION_SUPPLIED);

            int ptrMessageSize = Marshal.SizeOf(Message3);
            String8 bytMessage = new String8(ptrMessageSize + LMChallenge.length + NTLMChallenge.length + UnicodeDomain.length + UnicodeUsername.length);
            IntPtr ipMessage3 = Marshal.AllocHGlobal(Marshal.SizeOf(Message3Client));
            try
            {
                Marshal.StructureToPtr(Message3Client, ipMessage3, false);
                for (int i = 0; i < ptrMessageSize; i++)
                {
                    bytMessage.bytes[i] = Marshal.ReadByte(ipMessage3, i);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(ipMessage3);
            }

            bytMessage.length = M3Len;
            bytMessage.append(LMChallenge.bytes);
            bytMessage.append(NTLMChallenge.bytes);
            bytMessage.append(UnicodeDomain.bytes);
            bytMessage.append(UnicodeUsername.bytes);

            return bytMessage;
        }

        public override state AcceptSecurityContext(String8 data, String8 ip)
        {
            int NTLMSSPMessageType3Size = Marshal.SizeOf<NTLMSSPMessageType3>();
            data = StringExtensions.ToLiteral(data);

            if (data.length < NTLMSSPMessageType3Size) { return state.SSP_FAILED; }


            IntPtr pBuf = IntPtr.Zero;
            byte[] NTLM3Header = new byte[NTLMSSPMessageType3Size];
            Buffer.BlockCopy(data.bytes, 0, NTLM3Header, 0, NTLMSSPMessageType3Size);


            try
            {
                pBuf = Marshal.AllocHGlobal(NTLMSSPMessageType3Size);
                Marshal.Copy(NTLM3Header, 0, pBuf, NTLMSSPMessageType3Size);
                Message3 = (NTLMSSPMessageType3)Marshal.PtrToStructure<NTLMSSPMessageType3>(pBuf);
            }
            catch (Exception e)
            {
                return state.SSP_FAILED;
            }
            finally
            {
                if (pBuf != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pBuf);
                }
            }



            ClientFlags = Message3.Flags;

            if ((IsNTLMSupported()) && (Message3.Type == NTLMTypeAuthentication))
            {
                if (Message3.UserName.Offset + Message3.UserName.Length <= data.length)
                {
                    ClientUsername = new String8(data.bytes, Message3.UserName.Offset, Message3.UserName.Offset + Message3.UserName.Length);
                }
                else { return state.SSP_FAILED; }

                if (Message3.TargetName.Offset + Message3.TargetName.Length <= data.length)
                {
                    ClientDomain = new String8(data.bytes, Message3.TargetName.Offset, Message3.TargetName.Offset + Message3.TargetName.Length);
                }
                else { return state.SSP_FAILED; }

                if (Message3.LMResponse.Offset + Message3.LMResponse.Length <= data.length)
                {
                    LMResponse = new String8(data.bytes, Message3.LMResponse.Offset, Message3.LMResponse.Offset + Message3.LMResponse.Length);
                }
                else { return state.SSP_FAILED; }

                if (Message3.NTLMResponse.Offset + Message3.NTLMResponse.Length <= data.length)
                {
                    NTLMResponse = new String8(data.bytes, Message3.NTLMResponse.Offset, Message3.NTLMResponse.Offset + Message3.NTLMResponse.Length);
                }
                else { return state.SSP_FAILED; }

                //account User = accounts.GetUser(ClientDomain.ToUpper(), ClientUsername.ToUpper());
                //User u = null;
                //if (u != null)
                ClientUsername = new String8(String8.fromutf16(ClientUsername.bytes));
                //UserCredentials = Credentials.GetCredentials(ClientUsername);
                if (UserCredentials != null)
                {
                    if (UserCredentials.Password != null)
                    {
                        uuid = ClientUsername.bytes;
                        if (VerifySecurityContext(Challenge, UserCredentials.Password))
                        {
                            server_sequence = (int)state.SSP_AUTHENTICATED;
                            IsAuthenticated = true;
                            return state.SSP_OK;
                        }
                        else { return state.SSP_FAILED; }
                    }
                    else { return state.SSP_FAILED; }
                }
                else { return state.SSP_FAILED; }
            }
            else { return state.SSP_FAILED; }

            //return state.SSP_OK;

        }

        private bool VerifySecurityContext(String8 challenge, String8 password)
        {

            //Process LM Hash
            String8 LMpassword = new String8(14);
            LMpassword.append(password);
            LMpassword.ToUpper();

            String8 key1 = new String8(LMpassword.bytes, 0, 7);
            String8 key2 = new String8(LMpassword.bytes, 7, 14);

            /* Encrypt 14 byte password */
            String8 hash1 = Des.Encrypt(NTLM_DES_CONST, key1);
            String8 hash2 = Des.Encrypt(NTLM_DES_CONST, key2);

            if ((hash1 == null) || (hash2 == null)) return false;

            String8 LMPasswordHash = new String8(16);
            LMPasswordHash.append(hash1);
            LMPasswordHash.append(hash2);
            /* End of LM Hash Processing */

            /* Process NTLM Hash */
            MD4 md4 = new MD4();
            byte[] unicodePassword = String8.toutf16(password.bytes);
            md4.HashCore(unicodePassword, 0, unicodePassword.Length);
            byte[] MD4Hash = md4.HashFinal();

            String8 NTLMPasswordHash = new String8(MD4Hash);

            LMChallenge = CalculateLMHash(LMPasswordHash, challenge);
            NTLMChallenge = CalculateLMHash(NTLMPasswordHash, challenge);

            switch (NTLMOptions)
            {
                case NTLMOption.LMAuthOnly:
                    {
                        if (LMResponse == LMChallenge) { return true; }
                        else { return false; }
                    }
                case NTLMOption.NTLMAuthOnly:
                    {
                        if (NTLMResponse == NTLMChallenge) { return true; }
                        else { return false; }
                    }
                case NTLMOption.NTLM:
                    {
                        if ((LMResponse == LMChallenge) && (NTLMResponse == NTLMChallenge)) { return true; } // Note this wont with with CAC as both responses come back as NTLMChallenge, not LMChallenge
                        else { return false; }
                    }
            }
            return false;
        }
        private String8 CalculateLMHash(String8 hash, String8 challenge)
        {
            /* Join + Pad to 21 byte hash */
            String8 hash3 = new String8(21);
            hash3.append(hash);

            /* Split into 3 7 byte hashes */
            String8 hash4 = new String8(hash3.bytes, 0, 7);
            String8 hash5 = new String8(hash3.bytes, 7, 14);
            String8 hash6 = new String8(hash3.bytes, 14, 21);

            String8 des1 = Des.Encrypt(challenge, hash4);
            String8 des2 = Des.Encrypt(challenge, hash5);
            String8 des3 = Des.Encrypt(challenge, hash6);

            String8 result = new String8(24);
            result.append(des1);
            result.append(des2);
            result.append(des3);
            return result;
        }

    }
}
