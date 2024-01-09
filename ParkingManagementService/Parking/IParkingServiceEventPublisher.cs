﻿using ErrorOr;
using Kafka.Events.Contracts.Parking.Management;

namespace ParkingManagementService.Parking;

public interface IParkingServiceEventPublisher
{
    Task<ErrorOr<Success>> PublishParkingAddedAsync(ParkingAddedEvent parkingAddedEvent);
    Task<ErrorOr<Success>> PublishParkingUpdatedAsync(ParkingUpdatedEvent parkingUpdatedEvent);
    Task<ErrorOr<Success>> PublishParkingDeletedAsync(ParkingDeletedEvent parkingDeletedEvent);
}