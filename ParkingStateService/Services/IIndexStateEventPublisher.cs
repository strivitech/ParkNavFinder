using ErrorOr;
using ParkingStateService.Events;

namespace ParkingStateService.Services;

public interface IIndexStateEventPublisher
{
    Task<ErrorOr<Success>> PublishStateChangedAsync(IndexStateChangedEvent indexStateChangedEvent);
}