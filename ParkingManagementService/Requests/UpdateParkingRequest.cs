using ParkingManagementService.Models;

namespace ParkingManagementService.Requests;

public record UpdateParkingRequest( 
    Guid Id,
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    int TotalSpaces);