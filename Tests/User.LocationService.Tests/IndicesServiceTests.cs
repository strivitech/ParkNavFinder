using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StackExchange.Redis;
using User.LocationService.Services;

namespace User.LocationService.Tests;

public class IndicesServiceTests
{
    private readonly IDatabase _database;
    private readonly IndicesService _service;

    public IndicesServiceTests()
    {
        var connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
        _database = Substitute.For<IDatabase>();
        connectionMultiplexer.GetDatabase().Returns(_database);
        var logger = Substitute.For<ILogger<IndicesService>>();
        
        _service = new IndicesService(connectionMultiplexer, logger);
    }

    [Fact]
    public async Task GetUsersAttachedToIndexAsync_WhenNoUsers_ReturnsEmptyList()
    {
        // Arrange
        const string index = "testIndex";
        _database.SetMembersAsync(index, Arg.Any<CommandFlags>()).Returns(Array.Empty<RedisValue>());

        // Act
        var result = await _service.GetUsersAttachedToIndexAsync(index);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsersAttachedToIndexAsync_WhenUsersExist_ReturnsListOfUsers()
    {
        // Arrange
        const string index = "testIndex";
        var users = new RedisValue[] { "user1", "user2", "user3" };
        _database.SetMembersAsync(index, Arg.Any<CommandFlags>()).Returns(users);

        // Act
        var result = await _service.GetUsersAttachedToIndexAsync(index);

        // Assert
        result.Should().HaveCount(users.Length);
        result.Should().BeEquivalentTo(users.Select(u => u.ToString()));
    }
}