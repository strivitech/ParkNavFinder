namespace Parking.StateService.Domain;

// Note that some properties are used with BulkUpdateAsync extension method with BulkConfig { PropertiesToExclude }.
// This is because the GeoIndex property is not updated by the ParkingAnalyticsChangedEvent.
// Currently, adding new properties to the ParkingState class requires updating the BulkUpdateAsync call in the ParkingAnalyticsChangedEventHandler.
public class ParkingState
{
    public string ParkingId { get; set; } = null!;
    
    public string GeoIndex { get; set; } = null!;
    
    public int TotalObservers { get; set; }
    
    public double Probability { get; set; }
    
    public DateTime LastCalculatedUtc { get; set; }
}