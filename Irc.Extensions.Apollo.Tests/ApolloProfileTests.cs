using Irc.Extensions.Apollo.Objects.User;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irc.Extensions.Apollo.Tests
{
    public class ApolloProfileTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ApolloProfileTests_GetProfileStringTests()
        {
            ApolloProfile fy = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = true,
                IsMale = false,
                IsFemale = true
            };

            ClassicAssert.AreEqual(13, fy.GetProfileCode());
            ClassicAssert.AreEqual("FY", fy.GetProfileString());

            ApolloProfile my = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = true,
                IsMale = true,
                IsFemale = false
            };

            ClassicAssert.AreEqual(11, my.GetProfileCode());
            ClassicAssert.AreEqual("MY", my.GetProfileString());

            ApolloProfile py = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = true,
                IsMale = false,
                IsFemale = false
            };

            ClassicAssert.AreEqual(9, py.GetProfileCode());
            ClassicAssert.AreEqual("PY", py.GetProfileString());

            ApolloProfile fx = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = false,
                IsMale = false,
                IsFemale = true
            };

            ClassicAssert.AreEqual(5, fx.GetProfileCode());
            ClassicAssert.AreEqual("FX", fx.GetProfileString());

            ApolloProfile mx = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = false,
                IsMale = true,
                IsFemale = false
            };

            ClassicAssert.AreEqual(3, mx.GetProfileCode());
            ClassicAssert.AreEqual("MX", mx.GetProfileString());

            ApolloProfile px = new ApolloProfile()
            {
                HasProfile = true,
                HasPicture = false,
                IsMale = false,
                IsFemale = false
            };

            ClassicAssert.AreEqual(1, px.GetProfileCode());
            ClassicAssert.AreEqual("PX", px.GetProfileString());

            ApolloProfile rx = new ApolloProfile()
            {
                HasProfile = false,
                HasPicture = false,
                IsMale = false,
                IsFemale = false
            };

            ClassicAssert.AreEqual(0, rx.GetProfileCode());
            ClassicAssert.AreEqual("RX", rx.GetProfileString());
        }

        [Test]
        public void ApolloProfileTests_GetModeStringTests()
        {
            ApolloProfile admin = new ApolloProfile()
            {
                Level = Enumerations.EnumUserAccessLevel.Administrator
            };

            ClassicAssert.AreEqual("A", admin.GetModeString());

            ApolloProfile sysop = new ApolloProfile()
            {
                Level = Enumerations.EnumUserAccessLevel.Sysop
            };

            ClassicAssert.AreEqual("S", sysop.GetModeString());

            ApolloProfile user = new ApolloProfile()
            {
                Level = Enumerations.EnumUserAccessLevel.Member
            };

            ClassicAssert.AreEqual("U", user.GetModeString());
        }

        [Test]
        public void ApolloProfileTests_GetAwayStringTests()
        {
            ApolloProfile gone = new ApolloProfile()
            {
                Away = true
            };
            ClassicAssert.AreEqual("G", gone.GetAwayString());

            ApolloProfile here = new ApolloProfile()
            {
                Away = false
            };
            ClassicAssert.AreEqual("H", here.GetAwayString());
        }

        [Test]
        public void ApolloProfileTests_ToString()
        {
            ApolloProfile here_admin_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Administrator,
                Guest = true
            };
            ClassicAssert.AreEqual("H,A,GO", here_admin_guest.ToString());

            ApolloProfile here_user_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = true
            };
            ClassicAssert.AreEqual("H,U,GO", here_user_guest.ToString());

            ApolloProfile away_user_male_prof_registered = new ApolloProfile()
            {
                Away = true,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = false,
                HasProfile = true,
                IsMale = true,
                Registered = true
            };
            ClassicAssert.AreEqual("G,U,MXB", away_user_male_prof_registered.ToString());

            ApolloProfile away_user_female_prof_pic_registered = new ApolloProfile()
            {
                Away = true,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = false,
                HasProfile = true,
                IsMale = false,
                IsFemale = true,
                HasPicture = true,
                Registered = true
            };
            ClassicAssert.AreEqual("G,U,FYB", away_user_female_prof_pic_registered.ToString());
        }

        [Test]
        public void ApolloProfileTests_Irc5_ToString()
        {
            ApolloProfile here_admin_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Administrator,
                Guest = true
            };
            ClassicAssert.AreEqual("H,A,G", here_admin_guest.Irc5_ToString());

            ApolloProfile here_user_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = true
            };
            ClassicAssert.AreEqual("H,U,G", here_user_guest.Irc5_ToString());

            ApolloProfile away_user_male_prof_registered = new ApolloProfile()
            {
                Away = true,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = false,
                HasProfile = true,
                IsMale = true,
                Registered = true
            };
            ClassicAssert.AreEqual("G,U,M", away_user_male_prof_registered.Irc5_ToString());
        }

        [Test]
        public void ApolloProfileTests_Irc7_ToString()
        {
            ApolloProfile here_admin_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Administrator,
                Guest = true
            };
            ClassicAssert.AreEqual("H,A,G", here_admin_guest.Irc7_ToString());

            ApolloProfile here_user_guest = new ApolloProfile()
            {
                Away = false,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = true
            };
            ClassicAssert.AreEqual("H,U,G", here_user_guest.Irc7_ToString());

            ApolloProfile away_user_male_prof_registered = new ApolloProfile()
            {
                Away = true,
                Level = Enumerations.EnumUserAccessLevel.Member,
                Guest = false,
                HasProfile = true,
                IsMale = true,
                Registered = true
            };
            ClassicAssert.AreEqual("G,U,MX", away_user_male_prof_registered.Irc7_ToString());
        }
    }
}
