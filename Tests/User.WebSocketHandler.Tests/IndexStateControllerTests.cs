using FluentAssertions;
using Kafka.Events.Contracts.Parking.State;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using User.WebSocketHandler.Contracts;
using User.WebSocketHandler.Controllers;
using User.WebSocketHandler.Services;

namespace User.WebSocketHandler.Tests;

public class IndexStateControllerTests
{
    private readonly IIndexStateNotificationService _mockIndexStateNotificationService;
    private readonly IndexStateController _controller;

    public IndexStateControllerTests()
    {
        _mockIndexStateNotificationService = Substitute.For<IIndexStateNotificationService>();
        _controller = new IndexStateController(_mockIndexStateNotificationService);
    }

    [Fact]
    public async Task Notify_WhenCalled_ReturnsOkResult()
    {
        // Arrange
        var indexStateNotification = new IndexStateNotification
        {
            ReceiverIds = ["User1", "User2"],
            State = [new ParkingStateInfo("ParkingA", 10, 100, 75, DateTime.UtcNow)]
        };

        // Act
        var result = await _controller.Notify(indexStateNotification);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Notify_WhenCalled_InvokesNotifyUsersAsyncOnIndexStateNotificationService()
    {
        // Arrange
        var indexStateNotification = new IndexStateNotification
        {
            ReceiverIds = ["User1", "User2"],
            State = [new ParkingStateInfo("ParkingA", 10, 100, 75, DateTime.UtcNow)]
        };

        // Act
        await _controller.Notify(indexStateNotification);

        // Assert
        await _mockIndexStateNotificationService.Received(1)
            .NotifyUsersAsync(indexStateNotification.ReceiverIds, indexStateNotification.State);
    }
}