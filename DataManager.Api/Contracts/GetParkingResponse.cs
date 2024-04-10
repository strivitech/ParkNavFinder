namespace DataManager.Api.Contracts;

public record GetParkingResponse(
    Guid Id,
    string ProviderId,
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    string GeoIndex,
    int TotalSpaces);