namespace ParkingManagementService.Events;

public record ParkingAddedEvent(Guid ParkingId, double Latitude, double Longitude, DateTime CreatedAt);