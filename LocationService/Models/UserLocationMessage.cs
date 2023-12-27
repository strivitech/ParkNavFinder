using LocationService.Requests;

namespace LocationService.Models;

public record UserLocationMessage
{
    public string UserId { get; init; } = null!;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime Timestamp { get; init; }

    public static UserLocationMessage FromPostUserLocationRequest(PostUserLocationRequest postUserLocationRequest)
    {
        return new UserLocationMessage
        {
            UserId = postUserLocationRequest.UserId,
            Latitude = postUserLocationRequest.Latitude,
            Longitude = postUserLocationRequest.Longitude,
            Timestamp = DateTime.UtcNow
        };
    }
}