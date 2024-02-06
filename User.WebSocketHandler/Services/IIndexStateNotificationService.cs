using Kafka.Events.Contracts.Parking.State;

namespace User.WebSocketHandler.Services;

public interface IIndexStateNotificationService
{
    Task NotifyUsersAsync(List<string> userIds, List<ParkingStateInfo> parkingStateInfos);
}