namespace UserActiveGeoIndexService.Events;

public record UserLocationChangedEvent(string UserId, double Latitude, double Longitude, DateTime Timestamp);