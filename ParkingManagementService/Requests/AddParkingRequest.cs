using ParkingManagementService.Models;

namespace ParkingManagementService.Requests;

public record AddParkingRequest(
    string Name,
    string Description,
    Address Address,
    double Latitude,
    double Longitude,
    int TotalSpaces);