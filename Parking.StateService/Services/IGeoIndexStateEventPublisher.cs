using ErrorOr;
using Kafka.Events.Contracts.Parking.State;

namespace Parking.StateService.Services;

public interface IGeoIndexStateEventPublisher
{
    Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent);
}