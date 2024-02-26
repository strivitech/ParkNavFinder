namespace Auth.Shared;

public static class ApiKeyConstants
{
    public const string ConfigSectionName = "ApiKeys";
    public const string OwnApiKeyName = "OwnApiKey";      
    public const string HeaderName = "X-Api-Key";
    
    public const string UserWebSocketHandler = nameof(UserWebSocketHandler);
    public const string MapService = nameof(MapService);
    public const string UserLocationService = nameof(UserLocationService);
    public const string ParkingStateService = nameof(ParkingStateService);
    public const string UserNotificationService = nameof(UserNotificationService);
    public const string DataManagerApi = nameof(DataManagerApi);
}