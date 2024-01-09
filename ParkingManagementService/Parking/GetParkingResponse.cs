namespace ParkingManagementService.Parking;

public record GetParkingResponse(
    Guid Id,
    string ProviderId,
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    int TotalSpaces);