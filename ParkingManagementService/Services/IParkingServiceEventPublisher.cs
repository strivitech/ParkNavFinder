using ErrorOr;
using ParkingManagementService.Events;

namespace ParkingManagementService.Services;

public interface IParkingServiceEventPublisher
{
    Task<ErrorOr<Success>> PublishParkingAddedAsync(ParkingAddedEvent parkingAddedEvent);
    Task<ErrorOr<Success>> PublishParkingUpdatedAsync(ParkingUpdatedEvent parkingUpdatedEvent);
    Task<ErrorOr<Success>> PublishParkingDeletedAsync(ParkingDeletedEvent parkingDeletedEvent);
}