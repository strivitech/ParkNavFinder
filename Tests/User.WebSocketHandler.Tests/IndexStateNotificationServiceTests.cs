using Kafka.Events.Contracts.Parking.State;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using User.WebSocketHandler.Hubs;
using User.WebSocketHandler.Services;

namespace User.WebSocketHandler.Tests;

public class IndexStateNotificationServiceTests
{
    private readonly IHubContext<UsersHub, IUsersClient> _mockHubContext;
    private readonly IUsersClient _mockUsersClient;
    private readonly IndexStateNotificationService _service;

    public IndexStateNotificationServiceTests()
    {
        _mockHubContext = Substitute.For<IHubContext<UsersHub, IUsersClient>>();
        _mockUsersClient = Substitute.For<IUsersClient>();
        _service = new IndexStateNotificationService(_mockHubContext);
    }

    [Fact]
    public async Task NotifyUsersAsync_WithValidData_CallsReceiveParkingState()
    {
        // Arrange
        var userIds = new List<string> { "User1", "User2" };
        var parkingStateInfos = new List<ParkingStateInfo>
        {
            new("ParkingA", 10, 0.75, DateTime.UtcNow)
        };

        _mockHubContext.Clients.Users(Arg.Is<List<string>>(ids => ids.SequenceEqual(userIds))).Returns(_mockUsersClient);

        // Act
        await _service.NotifyUsersAsync(userIds, parkingStateInfos);

        // Assert
        await _mockUsersClient.Received(1).ReceiveParkingState(parkingStateInfos);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public async Task NotifyUsersAsync_WithEmptyUserIdsOrParkingStateInfos_DoesNotCallReceiveParkingState(int userCount, int parkingInfoCount)
    {
        // Arrange
        var userIds = new List<string>(new string[userCount]);
        var parkingStateInfos = new List<ParkingStateInfo>(new ParkingStateInfo[parkingInfoCount]);

        // Act
        await _service.NotifyUsersAsync(userIds, parkingStateInfos);

        // Assert
        await _mockUsersClient.DidNotReceiveWithAnyArgs().ReceiveParkingState(parkingStateInfos);
    }
}