namespace ParkingStateService.Models;

public class ActiveIndex
{
    public string Index { get; set; } = null!;
    
    public DateTime LastUpdatedUtc { get; set; }
}