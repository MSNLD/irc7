using System;
using Irc.Extensions.Apollo.Security.Packages;
using Irc.Extensions.Security;
using Irc.Extensions.Security.Packages;
using Irc.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Irc.Extensions.Apollo.Tests;

public class GateKeeperTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AcceptSecurityContext_V1_Auth_Fails_If_Guid_Exists()
    {
        var gateKeeper = new GateKeeper();
        var gateKeeperToken = new GateKeeperToken();
        gateKeeperToken.Signature = "GKSSP\0".ToByteArray();
        gateKeeperToken.Version = 1;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_INIT;

        var token = $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}";

        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.InitializeSecurityContext(token, null));

        gateKeeperToken.Version = 1;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_EXT;

        token =
            $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}{new Guid().ToByteArray().ToAsciiString()}{new Guid().ToByteArray().ToAsciiString()}";
        gateKeeper.SetChallenge(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_FAILED, gateKeeper.AcceptSecurityContext(token, null));
    }

    [Test]
    public void AcceptSecurityContext_Auth_Fails_If_Guest_And_Guid_Blank()
    {
        var gateKeeper = new GateKeeper();
        gateKeeper.Guest = true;

        var gateKeeperToken = new GateKeeperToken();
        gateKeeperToken.Signature = "GKSSP\0".ToByteArray();
        gateKeeperToken.Version = 2;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_INIT;

        var token = $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}";

        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.InitializeSecurityContext(token, null));

        gateKeeperToken.Version = 2;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_EXT;

        // Below contains magical answer guid to null byte challenge
        token =
            $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}{Guid.Parse("e23a3251-b322-8b2b-a34c-c4d0be30c5dd").ToByteArray().ToAsciiString()}{new Guid().ToByteArray().ToAsciiString()}";
        gateKeeper.SetChallenge(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        gateKeeper.CreateSecurityChallenge();
        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_FAILED, gateKeeper.AcceptSecurityContext(token, null));
    }

    [Test]
    public void AcceptSecurityContext_Auth_Succeeds_If_Not_Guest_And_Guid_Blank()
    {
        var gateKeeper = new GateKeeper();
        gateKeeper.Guest = false;

        var gateKeeperToken = new GateKeeperToken();
        gateKeeperToken.Signature = "GKSSP\0".ToByteArray();
        gateKeeperToken.Version = 2;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_INIT;

        var token = $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}";

        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.InitializeSecurityContext(token, null));

        gateKeeperToken.Version = 2;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_EXT;

        // Below contains magical answer guid to null byte challenge
        token =
            $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}{Guid.Parse("e23a3251-b322-8b2b-a34c-c4d0be30c5dd").ToByteArray().ToAsciiString()}{new Guid().ToByteArray().ToAsciiString()}";
        gateKeeper.SetChallenge(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        gateKeeper.CreateSecurityChallenge();
        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.AcceptSecurityContext(token, null));
    }

    [Test]
    public void AcceptSecurityContext_V3_Auth_Succeeds_With_IP()
    {
        var ip = "1.2.3.4";

        var gateKeeper = new GateKeeper();
        gateKeeper.Guest = false;

        var gateKeeperToken = new GateKeeperToken();
        gateKeeperToken.Signature = "GKSSP\0".ToByteArray();
        gateKeeperToken.Version = 3;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_INIT;

        var token = $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}";

        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.InitializeSecurityContext(token, null));

        gateKeeperToken.Version = 3;
        gateKeeperToken.Sequence = (int)EnumSupportPackageSequence.SSP_EXT;

        // Below contains magical answer guid to null byte challenge with ip
        token =
            $"{gateKeeperToken.Serialize<GateKeeperToken>().ToAsciiString()}{Guid.Parse("a8b9a59e-bd4d-411d-7728-4ec15d29282b").ToByteArray().ToAsciiString()}{new Guid().ToByteArray().ToAsciiString()}";
        gateKeeper.SetChallenge(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
        gateKeeper.CreateSecurityChallenge();
        ClassicAssert.AreEqual(EnumSupportPackageSequence.SSP_OK, gateKeeper.AcceptSecurityContext(token, ip));
    }
}