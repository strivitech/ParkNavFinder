namespace Kafka.Events.Contracts.Parking.Management;

public record ParkingUpdatedEvent(Guid ParkingId, int TotalSpaces);