using Kafka.Events.Contracts.Parking.State;
using Microsoft.AspNetCore.SignalR;
using User.WebSocketHandler.Hubs;

namespace User.WebSocketHandler.Services;

public class IndexStateNotificationService(IHubContext<UsersHub, IUsersClient> hub)
    : IIndexStateNotificationService
{
    private readonly IHubContext<UsersHub, IUsersClient> _hub = hub;
    
    public async Task NotifyUsersAsync(List<string> userIds, List<ParkingStateInfo> parkingStateInfos)
    {
        if (userIds.Count == 0 || parkingStateInfos.Count == 0)
        {
            return;
        }
        
        await _hub.Clients.Users(userIds).ReceiveParkingState(parkingStateInfos);
    }
}