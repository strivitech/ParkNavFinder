using ParkingStateService.Models;

namespace ParkingStateService.DTOs;

public record ActiveParkingStateDto(
    string ParkingId,
    int TotalObservers,
    int TotalPlaces,
    int Probability,
    DateTime LastCalculatedUtc)
{
    public static ActiveParkingStateDto FromActiveParkingState(ActiveParkingState activeParkingState) =>
        new(
            activeParkingState.ParkingId,
            activeParkingState.TotalObservers,
            activeParkingState.TotalPlaces,
            activeParkingState.Probability,
            activeParkingState.LastCalculatedUtc);
}