﻿using System;
using System.Runtime.InteropServices;
using Irc.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Irc.Extensions.NTLM.Tests.MessageTests;

internal class NtlmType1MessageTests
{
    [Test]
    public void NtlmType1Message_BlankStringThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new NtlmType1Message(string.Empty));
    }

    [Test]
    public void NtlmType1Message_InsufficientMessageLengthThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new NtlmType1Message(new string('A', Marshal.SizeOf<NTLMShared.NTLMSSPMessageType1>() - 1)));
    }

    [Test]
    public void NtlmType1Message_DoesNotStartWithSignatureThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            new NtlmType1Message(new string('A', Marshal.SizeOf<NTLMShared.NTLMSSPMessageType1>())));
    }

    [Test]
    public void NtlmType1Message_CACDeserializesOK()
    {
        var ntlmMessage1 =
            new NtlmType1Message("NTLMSSP\0\u0001\0\0\0\a\u0082\b¢\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\n\0ðU\0\0\0\u000f");

        ClassicAssert.AreEqual(NTLMShared.NTLMSignature, ntlmMessage1.Signature.ToAsciiString());
        ClassicAssert.AreEqual(10, ntlmMessage1.ClientVersion.Major);
        ClassicAssert.AreEqual(ntlmMessage1.EnumeratedFlags[NtlmFlag.NTLMSSP_NEGOTIATE_NTLM2], true);
    }

    [Test]
    public void NtlmType1Message_CACDeserializeOKOn95IE3()
    {
        var ntlmMessage1 =
            new NtlmType1Message(
                "NTLMSSP\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\00\0\0\0\0\08\0\0\0username172.22.1.18:6667");
    }
}