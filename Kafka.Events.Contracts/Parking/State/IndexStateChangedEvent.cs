namespace Kafka.Events.Contracts.Parking.State;

public record IndexStateChangedEvent(
    string GeoIndex,   
    IList<ParkingStateInfo> ParkingStates
);  