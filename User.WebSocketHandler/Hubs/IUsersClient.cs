using Kafka.Events.Contracts.Parking.State;

namespace User.WebSocketHandler.Hubs;

public interface IUsersClient
{
    Task ReceiveParkingState(List<ParkingStateInfo> parkingStateInfos); 
}