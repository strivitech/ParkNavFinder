using Parking.ManagementService.Domain;

namespace Parking.ManagementService.Contracts;

public record GetParkingResponse(
    Guid Id,
    string ProviderId,
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    int TotalSpaces);