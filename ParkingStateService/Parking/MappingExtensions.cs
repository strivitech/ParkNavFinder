using Kafka.Events.Contracts.Parking.State;

namespace ParkingStateService.Parking;

public static class MappingExtensions
{
    public static ParkingState ToParkingState(this ParkingStateModel parkingStateModel) =>
        new(
            parkingStateModel.ParkingId,
            parkingStateModel.TotalObservers,
            parkingStateModel.TotalPlaces,
            parkingStateModel.Probability,
            parkingStateModel.LastCalculatedUtc);
}