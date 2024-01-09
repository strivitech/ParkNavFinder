using Kafka.Events.Contracts.Parking.State;
using ParkingStateService.Models;

namespace ParkingStateService.Common;

public static class MappingExtensions
{
    public static ParkingState ToParkingState(this ActiveParkingState activeParkingState) =>
        new(
            activeParkingState.ParkingId,
            activeParkingState.TotalObservers,
            activeParkingState.TotalPlaces,
            activeParkingState.Probability,
            activeParkingState.LastCalculatedUtc);
}