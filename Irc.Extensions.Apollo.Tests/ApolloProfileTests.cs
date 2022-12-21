using Irc.Extensions.Apollo.Objects.User;
using Irc.Models.Enumerations;
using NUnit.Framework;

namespace Irc.Extensions.Apollo.Tests;

public class ApolloProfileTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ApolloProfileTests_GetProfileStringTests()
    {
        var fy = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = true,
            IsMale = false,
            IsFemale = true
        };

        Assert.AreEqual(13, fy.GetProfileCode());
        Assert.AreEqual("FY", fy.GetProfileString());

        var my = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = true,
            IsMale = true,
            IsFemale = false
        };

        Assert.AreEqual(11, my.GetProfileCode());
        Assert.AreEqual("MY", my.GetProfileString());

        var py = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = true,
            IsMale = false,
            IsFemale = false
        };

        Assert.AreEqual(9, py.GetProfileCode());
        Assert.AreEqual("PY", py.GetProfileString());

        var fx = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = false,
            IsMale = false,
            IsFemale = true
        };

        Assert.AreEqual(5, fx.GetProfileCode());
        Assert.AreEqual("FX", fx.GetProfileString());

        var mx = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = false,
            IsMale = true,
            IsFemale = false
        };

        Assert.AreEqual(3, mx.GetProfileCode());
        Assert.AreEqual("MX", mx.GetProfileString());

        var px = new ApolloProfile
        {
            HasProfile = true,
            HasPicture = false,
            IsMale = false,
            IsFemale = false
        };

        Assert.AreEqual(1, px.GetProfileCode());
        Assert.AreEqual("PX", px.GetProfileString());

        var rx = new ApolloProfile
        {
            HasProfile = false,
            HasPicture = false,
            IsMale = false,
            IsFemale = false
        };

        Assert.AreEqual(0, rx.GetProfileCode());
        Assert.AreEqual("RX", rx.GetProfileString());
    }

    [Test]
    public void ApolloProfileTests_GetModeStringTests()
    {
        var admin = new ApolloProfile
        {
            Level = EnumUserAccessLevel.Administrator
        };

        Assert.AreEqual("A", admin.GetModeString());

        var sysop = new ApolloProfile
        {
            Level = EnumUserAccessLevel.Sysop
        };

        Assert.AreEqual("S", sysop.GetModeString());

        var user = new ApolloProfile
        {
            Level = EnumUserAccessLevel.Member
        };

        Assert.AreEqual("U", user.GetModeString());
    }

    [Test]
    public void ApolloProfileTests_GetAwayStringTests()
    {
        var gone = new ApolloProfile
        {
            Away = true
        };
        Assert.AreEqual("G", gone.GetAwayString());

        var here = new ApolloProfile
        {
            Away = false
        };
        Assert.AreEqual("H", here.GetAwayString());
    }

    [Test]
    public void ApolloProfileTests_ToString()
    {
        var here_admin_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Administrator,
            Guest = true
        };
        Assert.AreEqual("H,A,GO", here_admin_guest.ToString());

        var here_user_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Member,
            Guest = true
        };
        Assert.AreEqual("H,U,GO", here_user_guest.ToString());

        var away_user_male_prof_registered = new ApolloProfile
        {
            Away = true,
            Level = EnumUserAccessLevel.Member,
            Guest = false,
            HasProfile = true,
            IsMale = true,
            Registered = true
        };
        Assert.AreEqual("G,U,MXB", away_user_male_prof_registered.ToString());

        var away_user_female_prof_pic_registered = new ApolloProfile
        {
            Away = true,
            Level = EnumUserAccessLevel.Member,
            Guest = false,
            HasProfile = true,
            IsMale = false,
            IsFemale = true,
            HasPicture = true,
            Registered = true
        };
        Assert.AreEqual("G,U,FYB", away_user_female_prof_pic_registered.ToString());
    }

    [Test]
    public void ApolloProfileTests_Irc5_ToString()
    {
        var here_admin_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Administrator,
            Guest = true
        };
        Assert.AreEqual("H,A,G", here_admin_guest.Irc5_ToString());

        var here_user_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Member,
            Guest = true
        };
        Assert.AreEqual("H,U,G", here_user_guest.Irc5_ToString());

        var away_user_male_prof_registered = new ApolloProfile
        {
            Away = true,
            Level = EnumUserAccessLevel.Member,
            Guest = false,
            HasProfile = true,
            IsMale = true,
            Registered = true
        };
        Assert.AreEqual("G,U,M", away_user_male_prof_registered.Irc5_ToString());
    }

    [Test]
    public void ApolloProfileTests_Irc7_ToString()
    {
        var here_admin_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Administrator,
            Guest = true
        };
        Assert.AreEqual("H,A,G", here_admin_guest.Irc7_ToString());

        var here_user_guest = new ApolloProfile
        {
            Away = false,
            Level = EnumUserAccessLevel.Member,
            Guest = true
        };
        Assert.AreEqual("H,U,G", here_user_guest.Irc7_ToString());

        var away_user_male_prof_registered = new ApolloProfile
        {
            Away = true,
            Level = EnumUserAccessLevel.Member,
            Guest = false,
            HasProfile = true,
            IsMale = true,
            Registered = true
        };
        Assert.AreEqual("G,U,MX", away_user_male_prof_registered.Irc7_ToString());
    }
}