using Kafka.Events.Contracts.Parking.State;

namespace User.NotificationService.Contracts;

public class IndexStateNotification
{
    public IList<string> ReceiverIds { get; set; } = null!;
    public IList<ParkingStateInfo> State { get; set; } = null!; 
}