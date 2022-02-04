﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Irc.Extensions.NTLM.Tests.MessageTests
{
    internal class NtlmType3MessageTests
    {
        [Test]
        public void NtlmType3Message_CACDeserializezOKOn95IE3()
        {
            var ntlmMessage3 =
                new NtlmType3Message(
                    "NTLMSSP\0\0\0\0\0\0I\0\0\0\0\0\0\0a\0\0\0\0\04\0\0\0\0\0:\0\0\0\0\0B\0\0\0DOMAINusernameDEFAULTO:àK©hØóF¯¥QºóDØ(®}");
            bool b = ntlmMessage3.VerifySecurityContext("AAAAAAAA", "password");
        }
    }
}
