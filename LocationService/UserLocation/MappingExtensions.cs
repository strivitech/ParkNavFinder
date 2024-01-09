using Kafka.Events.Contracts.Location;

namespace LocationService.UserLocation;

public static class MappingExtensions
{
    public static UserLocationChangedEvent ToUserLocationChangedEvent(this PostUserLocationRequest postUserLocationRequest)
    {
        return new UserLocationChangedEvent
        {
            UserId = postUserLocationRequest.UserId,
            Latitude = postUserLocationRequest.Latitude,
            Longitude = postUserLocationRequest.Longitude,
            Timestamp = DateTime.UtcNow
        };
    }
}