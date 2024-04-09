namespace Parking.StateService.Domain;

public class ParkingState
{
    public string ParkingId { get; set; } = null!;
    
    public string GeoIndex { get; set; } = null!;
    
    public int TotalObservers { get; set; }
    
    public int TotalSpaces { get; set; }
    
    public double Probability { get; set; }
    
    public DateTime LastCalculatedUtc { get; set; }
}