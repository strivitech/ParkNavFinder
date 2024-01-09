namespace Kafka.Events.Contracts.Location;

public record UserLocationChangedEvent
{
    public string UserId { get; init; } = null!;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime Timestamp { get; init; }
}