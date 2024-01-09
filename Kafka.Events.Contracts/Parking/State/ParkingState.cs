namespace Kafka.Events.Contracts.Parking.State;

public record ParkingState(
    string ParkingId,
    int TotalObservers,
    int TotalPlaces,
    int Probability,
    DateTime LastCalculatedUtc);