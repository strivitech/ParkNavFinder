namespace ParkingStateService.SpatialIndex;

public interface IGeoIndexStateNotificationService
{
    Task NotifyWithParkingStatesAsync();
}