﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Irc.Helpers.CSharpTools;
using NUnit.Framework;

namespace Irc.Extensions.NTLM.Tests.MessageTests
{
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
            Assert.Throws<ArgumentException>(() => new NtlmType1Message(new string('A', Marshal.SizeOf<NTLMShared.NTLMSSPMessageType1>() - 1)));
        }

        [Test]
        public void NtlmType1Message_DoesNotStartWithSignatureThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new NtlmType1Message(new string('A', Marshal.SizeOf<NTLMShared.NTLMSSPMessageType1>())));
        }

        [Test]
        public void NtlmType1Message_CACDeserializesOK()
        {
            var ntlmMessage1 = new NtlmType1Message("NTLMSSP\0\u0001\0\0\0\a\u0082\b¢\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\n\0ðU\0\0\0\u000f");

            Assert.AreEqual(NTLMShared.NTLMSignature, ntlmMessage1.Signature.ToAsciiString());
            Assert.AreEqual(10, ntlmMessage1.ClientVersion.Major);
            Assert.AreEqual(ntlmMessage1.EnumeratedFlags[NtlmFlag.NTLMSSP_NEGOTIATE_NTLM2], true);
        }

        [Test]
        public void NtlmType1Message_CACDeserializeOKOn95IE3()
        {
            var ntlmMessage1 =
                new NtlmType1Message(
                    "NTLMSSP\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\00\0\0\0\0\08\0\0\0username172.22.1.18:6667");
        }
    }
}
