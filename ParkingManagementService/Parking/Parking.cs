namespace ParkingManagementService.Parking;

public class Parking
{
    public Guid Id { get; set; }
    public string ProviderId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public Address Address { get; set; } = null!;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TotalSpaces { get; set; }
}