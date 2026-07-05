using Irc.Enumerations;
using Irc.Interfaces;
using Irc.Objects.User;
using Moq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Irc.Tests.Objects.User;

[TestFixture]
public class UserFlushTests
{
    private Mock<IConnection> _mockConnection = null!;
    private Mock<IProtocol> _mockProtocol = null!;
    private Mock<IDataRegulator> _mockDataRegulator = null!;
    private Mock<IFloodProtectionProfile> _mockFloodProtectionProfile = null!;
    private Mock<IServer> _mockServer = null!;

    [SetUp]
    public void SetUp()
    {
        _mockConnection = new Mock<IConnection>();
        _mockProtocol = new Mock<IProtocol>();
        _mockDataRegulator = new Mock<IDataRegulator>();
        _mockFloodProtectionProfile = new Mock<IFloodProtectionProfile>();
        _mockServer = new Mock<IServer>();

        _mockProtocol.Setup(p => p.GetProtocolType()).Returns(EnumProtocolType.IRC8);
        _mockConnection.Setup(c => c.GetIp()).Returns("127.0.0.1");
        _mockServer.Setup(s => s.ToString()).Returns("TestServer");
        _mockServer.Setup(s => s.DisableGuestMode).Returns(false);
    }

    [Test]
    public void Flush_SendsFullMessage_WhenWithinMaxMessageLength()
    {
        const string msg = "HELLOWORLD";

        // Arrange
        _mockDataRegulator.Setup(d => d.GetOutgoingBytes()).Returns(msg.Length);
        _mockDataRegulator.Setup(d => d.GetOutgoingQueueLength()).Returns(1);
        _mockDataRegulator.Setup(d => d.PopOutgoing()).Returns(() => msg);

        _mockServer.Setup(s => s.MaxMessageLength).Returns(512);

        string? sent = null;
        _mockConnection.Setup(c => c.Send(It.IsAny<string>())).Callback<string>(s => sent = s);

        var user = new Irc.Objects.User.User(
            _mockConnection.Object,
            _mockProtocol.Object,
            _mockDataRegulator.Object,
            _mockFloodProtectionProfile.Object,
            _mockServer.Object,
            _ => throw new InvalidOperationException("SSPI not used in this test"))
        {
            Nickname = "Tester"
        };

        // Act
        user.Flush();

        // Assert
        Assert.That(sent, Is.Not.Null);
        Assert.That(sent, Is.EqualTo(msg + "\r\n"));
    }

    [Test]
    public void Flush_TruncatesAndLogsError_WhenExceedsMaxMessageLength()
    {
        // Create a long outgoing message
        var longMsg = new string('A', 50);

        _mockDataRegulator.Setup(d => d.GetOutgoingBytes()).Returns(longMsg.Length);
        _mockDataRegulator.Setup(d => d.GetOutgoingQueueLength()).Returns(1);
        _mockDataRegulator.Setup(d => d.PopOutgoing()).Returns(() => longMsg);

        // Very small MaxMessageLength to force truncation
        _mockServer.Setup(s => s.MaxMessageLength).Returns(10);

        string? sent = null;
        _mockConnection.Setup(c => c.Send(It.IsAny<string>())).Callback<string>(s => sent = s);

        // Configure NLog memory target to capture error logs
        var config = new LoggingConfiguration();
        var memoryTarget = new MemoryTarget { Name = "memory" };
        config.AddRuleForAllLevels(memoryTarget);
        LogManager.Configuration = config;

        var user = new Irc.Objects.User.User(
            _mockConnection.Object,
            _mockProtocol.Object,
            _mockDataRegulator.Object,
            _mockFloodProtectionProfile.Object,
            _mockServer.Object,
            _ => throw new InvalidOperationException("SSPI not used in this test"))
        {
            Nickname = "Tester"
        };

        // Act
        user.Flush();

        // Assert: sent must be truncated to MaxMessageLength - 2
        var expectedMax = _mockServer.Object.MaxMessageLength - 2;
        if (expectedMax < 0) expectedMax = 0;

        Assert.That(sent, Is.Not.Null);
        Assert.That(sent.Length, Is.LessThanOrEqualTo(expectedMax));
        // Confirm that memory target captured an Error-level message about truncation
        var logs = memoryTarget.Logs;
        Assert.That(logs, Has.Exactly(1).Matches<string>(l => l.Contains("exceeds Server.MaxMessageLength") || l.Contains("Truncating")));
    }
}



