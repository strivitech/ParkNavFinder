using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using User.LocationService.Controllers;
using User.LocationService.Services;

namespace User.LocationService.Tests;

public class IndicesControllerTests
{
    private readonly IndicesController _controller;
    private readonly IIndicesService _indicesService = Substitute.For<IIndicesService>();

    public IndicesControllerTests()
    {
        _controller = new IndicesController(_indicesService);
    }

    [Fact]
    public async Task GetUsersAttachedToIndex_WhenNoUsers_ReturnsNoContent()
    {
        // Arrange
        const string index = "testIndex";
        _indicesService.GetUsersAttachedToIndexAsync(index).Returns(Task.FromResult(new List<string>()));

        // Act
        var result = await _controller.GetUsersAttachedToIndex(index);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetUsersAttachedToIndex_WhenUsersExist_ReturnsOkObjectResultWithUsers()
    {
        // Arrange
        const string index = "testIndex";
        var users = new List<string> { "user1", "user2" };
        _indicesService.GetUsersAttachedToIndexAsync(index).Returns(Task.FromResult(users));

        // Act
        var result = await _controller.GetUsersAttachedToIndex(index);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(users);
    }
}