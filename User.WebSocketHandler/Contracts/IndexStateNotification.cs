using Kafka.Events.Contracts.Parking.State;

namespace User.WebSocketHandler.Contracts;

public class IndexStateNotification
{
    public List<string> ReceiverIds { get; set; } = null!;
    public List<ParkingStateInfo> State { get; set; } = null!;
}