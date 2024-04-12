namespace User.SelectParkingService.Contracts;

public record SelectParkingRequest(Coordinate UserPosition, Guid ParkingId);