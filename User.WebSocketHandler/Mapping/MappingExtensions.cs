using Kafka.Events.Contracts.User.Location;
using User.WebSocketHandler.Contracts;

namespace User.WebSocketHandler.Mapping;

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