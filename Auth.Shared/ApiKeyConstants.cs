namespace Auth.Shared;

public static class ApiKeyConstants
{
    public const string ConfigSectionName = "ApiKeys";
    public const string OwnApiKeyName = "OwnApiKey";      
    public const string HeaderName = "X-Api-Key";
    
    public const string ParkingOfferingService = nameof(ParkingOfferingService);
    public const string UserWebSocketHandler = nameof(UserWebSocketHandler);
    public const string MapService = nameof(MapService);
    public const string UserLocationService = nameof(UserLocationService);
}