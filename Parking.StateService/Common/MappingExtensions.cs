using ParkingState = Parking.StateService.Domain.ParkingState;

namespace Parking.StateService.Common;

public static class MappingExtensions
{
    public static Kafka.Events.Contracts.Parking.State.ParkingStateInfo ToParkingState(this ParkingState parkingState) =>
        new(
            parkingState.ParkingId,
            parkingState.TotalObservers,
            parkingState.Probability,
            parkingState.LastCalculatedUtc);
}