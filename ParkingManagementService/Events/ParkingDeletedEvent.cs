namespace ParkingManagementService.Events;

public record ParkingDeletedEvent(Guid ParkingId, DateTime DeletedAt);  