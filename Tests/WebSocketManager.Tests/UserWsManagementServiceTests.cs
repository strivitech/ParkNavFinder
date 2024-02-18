using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;
using WebSocketManager.Services;

namespace WebSocketManager.Tests;

public class UserWsManagementServiceTests
{
    private readonly IConnectionMultiplexer _mockMultiplexer = Substitute.For<IConnectionMultiplexer>();
    private readonly IDatabase _mockDatabase = Substitute.For<IDatabase>();
    private readonly UserWsManagementService _service;

    public UserWsManagementServiceTests()
    {
        _mockMultiplexer.GetDatabase().Returns(_mockDatabase);
        var mockLogger = Substitute.For<ILogger<UserWsManagementService>>();
        _service = new UserWsManagementService(_mockMultiplexer, mockLogger);
    }

    [Fact]
    public async Task GetHandlerHostAsync_WithValidUserId_ReturnsExpectedUri()
    {
        // Arrange
        const string userId = "validUserId";
        const string expectedUri = "ws://someuri.com";
        _mockDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns(expectedUri);

        // Act
        var result = await _service.GetHandlerHostAsync(userId);

        // Assert
        result.Should().Be(expectedUri);
    }

    [Fact]
    public async Task GetHandlerHostAsync_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        const string userId = "invalidUserId";
        _mockDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns((string?)null);

        // Act
        var result = await _service.GetHandlerHostAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetHandlerHostAsync_WithNullOrEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        const string userId = "";

        // Act
        Func<Task> act = async () => await _service.GetHandlerHostAsync(userId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetHandlerHostsAsync_WithValidUserIds_ReturnsExpectedUris()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2" };
        var expectedResults = new Dictionary<string, string?>
        {
            { "user1", "ws://user1uri.com" },
            { "user2", "ws://user2uri.com" }
        };

        _mockDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns("ws://user1uri.com", "ws://user2uri.com");

        // Act
        var result = await _service.GetHandlerHostsAsync(userIds);

        // Assert
        result.Should().BeEquivalentTo(expectedResults);
    }

    [Fact]
    public async Task GetHandlerHostsAsync_WithEmptyUserIdsList_ReturnsEmptyDictionary()
    {
        // Arrange
        var userIds = new List<string>();

        // Act
        var result = await _service.GetHandlerHostsAsync(userIds);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetHandlerHostsAsync_WithNullUserIdsList_ThrowsArgumentNullException()
    {
        // Arrange
        List<string> userIds = null!;

        // Act
        Func<Task> act = async () => await _service.GetHandlerHostsAsync(userIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SetHandlerAsync_WithValidParameters_SetsHandlerSuccessfully()
    {
        // Arrange
        const string userId = "validUserId";
        const string wsHandlerUri = "ws://someuri.com";
        _mockDatabase.StringSetAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), null, When.Always, CommandFlags.None)
            .Returns(true);

        // Act
        Func<Task> act = async () => await _service.SetHandlerAsync(userId, wsHandlerUri);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SetHandlerAsync_WithNullOrEmptyParameters_ThrowsArgumentException()
    {
        // Arrange
        const string userId = "";
        const string wsHandlerUri = "";

        // Act
        Func<Task> act = async () => await _service.SetHandlerAsync(userId, wsHandlerUri);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task RemoveHandlerAsync_WithValidUserId_RemovesHandlerSuccessfully()
    {
        // Arrange
        const string userId = "validUserId";
        _mockDatabase.KeyDeleteAsync(Arg.Any<RedisKey>()).Returns(true);

        // Act
        Func<Task> act = async () => await _service.RemoveHandlerAsync(userId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RemoveHandlerAsync_WithNullOrEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        const string userId = "";

        // Act
        Func<Task> act = async () => await _service.RemoveHandlerAsync(userId);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}