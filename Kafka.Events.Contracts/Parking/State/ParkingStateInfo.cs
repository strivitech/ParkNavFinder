namespace Kafka.Events.Contracts.Parking.State;

public record ParkingStateInfo(
    string ParkingId,
    int TotalObservers,
    int TotalPlaces,
    int Probability,
    DateTime LastCalculatedUtc);