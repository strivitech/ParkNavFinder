namespace Kafka.Events.Contracts.User.Location;

public record UserLocationChangedEvent
{
    public string UserId { get; init; } = null!;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}