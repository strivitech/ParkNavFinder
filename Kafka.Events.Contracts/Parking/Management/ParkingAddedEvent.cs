namespace Kafka.Events.Contracts.Parking.Management;

public record ParkingAddedEvent(Guid ParkingId, double Latitude, double Longitude, string GeoIndex, int TotalSpaces);