using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebSocketManager.Controllers;
using WebSocketManager.Services;

namespace WebSocketManager.Tests;

public class UserWsManagementControllerTests
{
    private readonly UserWsManagementController _controller;
    private readonly IUserWsManagementService _mockUserWsManagementService = Substitute.For<IUserWsManagementService>();

    public UserWsManagementControllerTests()
    {
        _controller = new UserWsManagementController(_mockUserWsManagementService);
    }
    
    [Fact]
    public async Task GetHandlerHost_ReturnsOk_WithValidUserId()
    {
        // Arrange
        const string userId = "validUserId";
        const string expectedUri = "ws://someuri.com";
        _mockUserWsManagementService.GetHandlerHostAsync(userId).Returns(expectedUri);

        // Act
        var result = await _controller.GetHandlerHost(userId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        okResult.Value.Should().Be(expectedUri);
    }
    
    [Fact]
    public async Task GetHandlerHost_ReturnsNotFound_WithInvalidUserId()
    {
        // Arrange
        const string userId = "invalidUserId";
        _mockUserWsManagementService.GetHandlerHostAsync(userId).Returns((string?)null);

        // Act
        var result = await _controller.GetHandlerHost(userId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async Task GetHandlerHosts_ReturnsOk_WithValidUserIds()
    {
        // Arrange
        var userIds = new List<string> { "user1", "user2" };
        var expectedUris = new Dictionary<string, string?> { { "user1", "ws://user1uri.com" }, { "user2", "ws://user2uri.com" } };
        _mockUserWsManagementService.GetHandlerHostsAsync(userIds).Returns(expectedUris);

        // Act
        var result = await _controller.GetHandlerHosts(userIds);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        okResult.Value.Should().BeEquivalentTo(expectedUris);
    }
    
    [Fact]
    public async Task GetHandlerHosts_ReturnsNoContent_WithEmptyUserIds()
    {
        // Arrange
        var userIds = new List<string>();
        _mockUserWsManagementService.GetHandlerHostsAsync(userIds).Returns(new Dictionary<string, string?>());

        // Act
        var result = await _controller.GetHandlerHosts(userIds);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async Task SetHandler_ReturnsOk_WithValidUserId()
    {
        // Arrange
        const string userId = "validUserId";

        // Act
        var result = await _controller.SetHandler(userId);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task RemoveHandler_ReturnsOk_WithValidUserId()
    {
        // Arrange
        const string userId = "validUserId";

        // Act
        var result = await _controller.RemoveHandler(userId);

        // Assert
        result.Should().BeOfType<OkResult>();
    }
}