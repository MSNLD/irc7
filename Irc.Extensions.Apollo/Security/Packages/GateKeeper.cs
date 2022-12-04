﻿using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Irc.ClassExtensions.CSharpTools;
using Irc.Enumerations;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Helpers.CSharpTools;
using Irc.Interfaces;
using Irc.Security;

// ReSharper disable once CheckNamespace
namespace Irc.Extensions.Security.Packages;

public class GateKeeper : SupportPackage, ISupportPackage
{
    private static readonly string _signature = "GKSSP\0";

    // Credit to JD for discovering the below key through XOR'ing (Discovered 2017/05/04)
    private static readonly string key = "SRFMKSJANDRESKKC";
    private readonly char[] challenge = new char[8];
    protected GateKeeperToken ServerToken;

    public GateKeeper()
    {
        Guest = true;
        ServerToken.Signature = _signature.ToByteArray();
        ServerSequence = EnumSupportPackageSequence.SSP_INIT;
    }

    public override SupportPackage CreateInstance(ICredentialProvider credentialProvider)
    {
        return new GateKeeper();
    }

    public override EnumSupportPackageSequence InitializeSecurityContext(string token, string ip)
    {
        // <byte(6) signature><byte(2)??><int(4) version><int(4) stage>
        if (token.Length >= 0x10)
            if (token.StartsWith(_signature))
            {
                var clientToken = GateKeeperTokenHelper.InitializeFromBytes(token.ToByteArray());
                if ((EnumSupportPackageSequence)clientToken.Sequence == EnumSupportPackageSequence.SSP_INIT &&
                    clientToken.Version is >= 1 and <= 3)
                {
                    ServerSequence = EnumSupportPackageSequence.SSP_EXT;
                    ServerVersion = clientToken.Version;
                    return EnumSupportPackageSequence.SSP_OK;
                }
            }

        return EnumSupportPackageSequence.SSP_FAILED;
    }

    public override EnumSupportPackageSequence AcceptSecurityContext(string token, string ip)
    {
        // <byte(6) signature><byte(2)??><int(4) version><int(4) stage><byte(16) challenge response><byte(16) guid>
        if (token.Length >= 0x20)
            if (token.StartsWith(_signature))
            {
                var clientToken = GateKeeperTokenHelper.InitializeFromBytes(token.ToByteArray());
                var clientVersion = clientToken.Version;
                var clientStage = (EnumSupportPackageSequence)clientToken.Sequence;

                if (clientStage != ServerSequence || clientVersion != ServerVersion)
                    return EnumSupportPackageSequence.SSP_FAILED;

                if (clientVersion == 1 && token.Length > 0x20) return EnumSupportPackageSequence.SSP_FAILED;

                var context = token.Substring(0x10, 0x10).ToByteArray();

                if (!VerifySecurityContext(new string(challenge), context, ip, ServerVersion))
                    return EnumSupportPackageSequence.SSP_FAILED;

                var guid = Guid.NewGuid();
                if (token.Length >= 0x30) guid = new Guid(token.Substring(0x20, 0x10).ToByteArray());

                if (guid != Guid.Empty || Guest == false)
                {
                    ServerSequence = EnumSupportPackageSequence.SSP_AUTHENTICATED;
                    Authenticated = true;

                    _credentials = new Credential
                    {
                        Level = Guest ? EnumUserAccessLevel.Guest : EnumUserAccessLevel.Member,
                        Domain = GetType().Name,
                        Username = guid.ToUnformattedString().ToUpper()
                    };

                    if (this is GateKeeperPassport) return EnumSupportPackageSequence.SSP_EXT;
                    return EnumSupportPackageSequence.SSP_OK;
                }
            }

        return EnumSupportPackageSequence.SSP_FAILED;
    }

    public override string CreateSecurityChallenge()
    {
        ServerToken.Sequence = (int)EnumSupportPackageSequence.SSP_SEC;
        ServerToken.Version = ServerVersion;
        Array.Copy(Guid.NewGuid().ToByteArray(), 0, challenge, 0, 8);
        //Array.Copy(new byte[] { 167, 135, 203, 141, 242, 118, 89, 77 }, 0, challenge, 0, 8);
        //Array.Copy(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 0, challenge, 0, 8);
        var message = new StringBuilder(Marshal.SizeOf(ServerToken) + challenge.Length);
        message.Append(ServerToken.Serialize<GateKeeperToken>().ToAsciiString());
        message.Append(challenge);
        return message.ToString();
    }

    private bool VerifySecurityContext(string challenge, byte[] context, string ip, uint version)
    {
        ip = version == 3 && ip != null ? ip : "";

        var md5 = new HMACMD5(key.ToByteArray());
        var ctx = $"{challenge}{ip}";
        var h1 = md5.ComputeHash(ctx.ToByteArray(), 0, ctx.Length);
        var b = h1.SequenceEqual(context);
        if (!b) return false;
        return b;
    }
}