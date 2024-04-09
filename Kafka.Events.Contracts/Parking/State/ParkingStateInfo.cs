namespace Kafka.Events.Contracts.Parking.State;

public record ParkingStateInfo(
    string ParkingId,
    int TotalObservers,
    double Probability, 
    DateTime LastCalculatedUtc);