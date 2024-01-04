namespace ParkingManagementService.Events;

public record ParkingUpdatedEvent(Guid ParkingId, DateTime UpdatedAt);