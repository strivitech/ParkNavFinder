namespace Kafka.Events.Contracts.Parking.State;

public record IndexStateChangedEvent(
    string EventId,     
    string Index,   
    IList<ParkingStateInfo> ParkingStates, 
    DateTime IssuedUtc
);  