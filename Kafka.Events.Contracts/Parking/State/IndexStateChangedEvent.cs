namespace Kafka.Events.Contracts.Parking.State;

public record IndexStateChangedEvent(
    string Index,   
    IList<ParkingStateInfo> ParkingStates
);  