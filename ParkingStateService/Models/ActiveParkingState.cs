namespace ParkingStateService.Models;

public class ActiveParkingState
{
    public string ParkingId { get; set; } = null!;
    
    public string Index { get; set; } = null!;
    
    public int TotalObservers { get; set; }
    
    public int TotalPlaces { get; set; }
    
    public int Probability { get; set; }
    
    public DateTime LastCalculatedUtc { get; set; }
}