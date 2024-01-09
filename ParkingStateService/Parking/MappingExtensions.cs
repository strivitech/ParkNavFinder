using Kafka.Events.Contracts.Parking.State;

namespace ParkingStateService.Parking;

public static class MappingExtensions
{
    public static ParkingState ToParkingState(this CurrentParkingState currentParkingState) =>
        new(
            currentParkingState.ParkingId,
            currentParkingState.TotalObservers,
            currentParkingState.TotalPlaces,
            currentParkingState.Probability,
            currentParkingState.LastCalculatedUtc);
}