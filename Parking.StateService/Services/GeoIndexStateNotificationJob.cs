﻿namespace Parking.StateService.Services;

public class GeoIndexStateNotificationJob(IGeoIndexStateNotificationService geoIndexStateNotificationService)
{
    private readonly IGeoIndexStateNotificationService _geoIndexStateNotificationService = geoIndexStateNotificationService;
    
    public async Task Complete() => await _geoIndexStateNotificationService.NotifyWithParkingStatesAsync();
}