namespace Parking.AnalyticsService.Domain;

public class Parking
{
    public string ParkingId { get; set; } = null!;
    
    public string GeoIndex { get; set; } = null!;
    
    public int TotalSpaces { get; set; }
    
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }
    
    public DateTime LastCalculatedUtc { get; set; }
}