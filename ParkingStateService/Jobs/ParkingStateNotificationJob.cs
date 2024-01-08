using ParkingStateService.Services;

namespace ParkingStateService.Jobs;

public class ParkingStateNotificationJob(IIndexStateNotificationService indexStateNotificationService)
{
    private readonly IIndexStateNotificationService _indexStateNotificationService = indexStateNotificationService;
    
    public async Task Complete() => await _indexStateNotificationService.NotifyWithParkingStatesAsync();
}