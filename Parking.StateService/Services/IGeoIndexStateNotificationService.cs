namespace Parking.StateService.Services;

public interface IGeoIndexStateNotificationService
{
    Task NotifyWithParkingStatesAsync();
}