namespace Kafka.Events.Contracts.Parking.State;

public record IndexStateChangedEvent(
    string EventId,     
    string Index,
    IEnumerable<ParkingState> ParkingState, 
    DateTime IssuedUtc
);  