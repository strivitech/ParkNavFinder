using ErrorOr;
using Kafka.Events.Contracts.Parking.State;

namespace ParkingStateService.SpatialIndex;

public interface IGeoIndexStateEventPublisher
{
    Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent);
}