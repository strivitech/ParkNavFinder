namespace ParkingStateService.Services;

public interface IIndexStateNotificationService
{
    Task NotifyWithParkingStatesAsync();
}