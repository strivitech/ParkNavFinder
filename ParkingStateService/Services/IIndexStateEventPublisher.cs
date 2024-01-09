using ErrorOr;
using Kafka.Events.Contracts.Parking.State;

namespace ParkingStateService.Services;

public interface IIndexStateEventPublisher
{
    Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent);
}