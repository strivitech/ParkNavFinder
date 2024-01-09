namespace Kafka.Events.Contracts.Parking.Management;

public record ParkingDeletedEvent(Guid ParkingId, DateTime DeletedAt);  