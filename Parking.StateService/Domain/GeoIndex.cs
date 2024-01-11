namespace Parking.StateService.Domain;

public class GeoIndex
{
    public string Index { get; set; } = null!;
    
    public DateTime LastUpdatedUtc { get; set; }
}