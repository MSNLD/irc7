using Irc.Access;
using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects.User;
using Moq;
using NUnit.Framework;

namespace Irc.Tests.Objects.User;

[TestFixture]
public class UserTests
{
    private Mock<IConnection> _mockConnection;
    private Mock<IProtocol> _mockProtocol;
    private Mock<IDataRegulator> _mockDataRegulator;
    private Mock<IFloodProtectionProfile> _mockFloodProtectionProfile;
    private Mock<IServer> _mockServer;
    private Func<bool, ISaslHandler> _saslHandlerFactory;

    [SetUp]
    public void SetUp()
    {
        _mockConnection = new Mock<IConnection>();
        _mockProtocol = new Mock<IProtocol>();
        _mockDataRegulator = new Mock<IDataRegulator>();
        _mockFloodProtectionProfile = new Mock<IFloodProtectionProfile>();
        _mockServer = new Mock<IServer>();
        _saslHandlerFactory = (passport) => new Mock<ISaslHandler>().Object;

        _mockConnection.Setup(c => c.GetIp()).Returns("127.0.0.1");
    }

    private Irc.Objects.User.User CreateUser(string nickname = "TestUser")
    {
        var user = new Irc.Objects.User.User(
            _mockConnection.Object,
            _mockProtocol.Object,
            _mockDataRegulator.Object,
            _mockFloodProtectionProfile.Object,
            _mockServer.Object,
            _saslHandlerFactory
        );
        user.Nickname = nickname;
        return user;
    }

    [Test]
    public void Grants_ReturnsTrue_WhenGrantListIsEmpty()
    {
        // Arrange
        var user = CreateUser();
        var targetUser = new Mock<IUser>();

        // Act
        var result = user.Grants(targetUser.Object);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Denys_ReturnsFalse_WhenDenyListIsEmpty()
    {
        // Arrange
        var user = CreateUser();
        var targetUser = new Mock<IUser>();

        // Act
        var result = user.Denys(targetUser.Object);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Grants_ReturnsTrue_WhenUserMatchesGrantEntry()
    {
        // Arrange
        var user = CreateUser();
        
        // We need to ensure GRANT level is available. Since UserAccess might not have it, 
        // we'll see if this test fails to even add the entry.
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("nick!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("nick!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.GRANT, "nick!user@host", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Grants(targetUser.Object);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Denys_ReturnsTrue_WhenUserMatchesDenyEntry()
    {
        // Arrange
        var user = CreateUser();
        
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("nick!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("nick!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.DENY, "nick!user@host", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Denys(targetUser.Object);

        // Assert
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void Grants_ReturnsFalse_WhenGrantListNotEmptyAndUserDoesNotMatch()
    {
        // Arrange
        var user = CreateUser();
        
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("other!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("other!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.GRANT, "nick!user@host", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Grants(targetUser.Object);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Denys_ReturnsFalse_WhenDenyListNotEmptyAndUserDoesNotMatch()
    {
        // Arrange
        var user = CreateUser();
        
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("other!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("other!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.DENY, "nick!user@host", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Denys(targetUser.Object);

        // Assert
        Assert.That(result, Is.False);
    }
    [Test]
    public void Grants_ReturnsTrue_WhenUserMatchesGrantEntryWithWildcard()
    {
        // Arrange
        var user = CreateUser();
        
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("nick!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("nick!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.GRANT, "*!*@host", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Grants(targetUser.Object);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Denys_ReturnsTrue_WhenUserMatchesDenyEntryWithWildcard()
    {
        // Arrange
        var user = CreateUser();
        
        var targetUser = new Mock<IUser>();
        var targetAddress = new Mock<IUserAddress>();
        targetAddress.Setup(a => a.GetFullAddress()).Returns("nick!user@host");
        targetAddress.Setup(a => a.GetIpFullAddress()).Returns("nick!user@127.0.0.1");
        targetUser.Setup(u => u.GetAddress()).Returns(targetAddress.Object);

        var entry = new AccessEntry("tester", EnumUserAccessLevel.None, EnumAccessLevel.DENY, "*!user@*", 0, "test");
        user.Access.Add(entry);

        // Act
        var result = user.Denys(targetUser.Object);

        // Assert
        Assert.That(result, Is.True);
    }
}
